using System;
using System.Linq;
using Dalamud.Logging;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Standards;
using MidiBard.Control.CharacterControl;
using MidiBard.HSC;
using MidiBard.Util;
using playlibnamespace;

namespace MidiBard.HSCM;

internal class BardPlayDevice : Control.BardPlayDevice
{



    /// <summary>
    /// Directly invoked by midi events sent from file playback
    /// </summary>
    /// <param name="midiEvent">Raw midi event</param>
    /// <param name="metadata">Currently is track index</param>
    /// <returns></returns>
    public new bool SendEventWithMetadata(MidiEvent midiEvent, object metadata)
    {
        if (!MidiBard.AgentPerformance.InPerformanceMode) return false;

        var trackIndex = (int?)metadata;
        if (trackIndex is { } trackIndexValue)
        {
            if (Configuration.config.SoloedTrack is { } soloing)
            {
                if (trackIndexValue != soloing)
                {
                    return false;
                }
            }
            else
            {
                if (!ConfigurationPrivate.config.EnabledTracks[trackIndexValue])
                {
                    return false;
                }

                if (Configuration.config.useHscmOverride && !HSC.PerformHelpers.ShouldPlayNote(midiEvent, trackIndexValue))
                    return false;
            }

            if (midiEvent is NoteOnEvent noteOnEvent)
            {
                if (MidiBard.PlayingGuitar)
                {
                    switch (Configuration.config.GuitarToneMode)
                    {
                        case GuitarToneMode.Off:
                            break;
                        case GuitarToneMode.Standard:
                            HandleToneSwitchEvent(noteOnEvent);
                            break;
                        case GuitarToneMode.Simple:
                            {
                                if (MidiBard.CurrentTracks[trackIndexValue].trackInfo.IsProgramControlled)
                                {
                                    HandleToneSwitchEvent(noteOnEvent);
                                }
                                break;
                            }
                        case GuitarToneMode.Override:
                            {
                                int tone = Configuration.config.TonesPerTrack[trackIndexValue];
                                playlib.GuitarSwitchTone(tone);

                                // PluginLog.Verbose($"[N][NoteOn][{trackIndex}:{noteOnEvent.Channel}] Overriding guitar tone {tone}");
                                break;
                            }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        return SendMidiEvent(midiEvent, trackIndex);
    }

    private static HSC.Settings.TrackTransposeInfo GetHSCTrackInfo(int trackIndex, int noteNumber)
    {
        if (HSC.Settings.MappedTracks.ContainsKey(trackIndex))
            return HSC.Settings.MappedTracks[trackIndex];

        if (!HSC.Settings.TrackInfo.ContainsKey(trackIndex))
            return null;

        return HSC.Settings.TrackInfo[trackIndex];
    }

    private static int TransposeFromHSCPlaylist(int noteNumber, int? trackIndex, bool plotting = false)
    {
        if (plotting)
            return 0;

        int noteNum = 0;

        var trackInfo = GetHSCTrackInfo(trackIndex.Value, noteNumber);

        if (trackInfo == null)
           return 0;

        if (trackInfo.OctaveOffset != 0)
            noteNum += (12 * trackInfo.OctaveOffset);

        if (trackInfo.KeyOffset != 0)
            noteNum += trackInfo.KeyOffset;

        return (12 * HSC.Settings.OctaveOffset) + HSC.Settings.KeyOffset + noteNum;
    }

    public static new int GetTranslatedNoteNum(int noteNumber, int? trackIndex, out int octave, bool plotting = false)
    {

        noteNumber = noteNumber - 48;

        octave = 0;

        if (Configuration.config.useHscmOverride && Configuration.config.useHscmTransposing)
            noteNumber += TransposeFromHSCPlaylist(noteNumber, trackIndex, plotting);
        else 
            noteNumber += Configuration.config.TransposeGlobal +
                         (Configuration.config.EnableTransposePerTrack && trackIndex is { } index ? Configuration.config.TransposePerTrack[index] : 0);

        if (Configuration.config.AdaptNotesOOR)
        {
            while (noteNumber < 0)
            {
                noteNumber += 12;
                octave++;
            }

            while (noteNumber > 36)
            {
                noteNumber -= 12;
                octave--;
            }
        }

        return noteNumber;
    }

    //bool GetKey( ,int midiNoteNumber, int trackIndex, out int key, out int octave)
    //{
    //	octave = 0;

    //	key = midiNoteNumber - 48 +
    //	          Configuration.config.TransposeGlobal +
    //	          (Configuration.config.EnableTransposePerTrack ? Configuration.config.TransposePerTrack[trackIndex] : 0);
    //	if (Configuration.config.AdaptNotesOOR)
    //	{
    //		while (key < 0)
    //		{
    //			key += 12;
    //			octave++;
    //		}
    //		while (key > 36)
    //		{
    //			key -= 12;
    //			octave--;
    //		}
    //	}


    //}
}