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

namespace MidiBard.HSCM.MidiControl;

internal class BardPlayDevice : Control.BardPlayDevice
{


    public BardPlayDevice() : base()
    {
        PluginLog.Information("Loaded HSCM bard play device");
    }

    /// <summary>
    /// Midi events send from input device
    /// </summary>
    /// <param name="midiEvent">Raw midi event</param>
    public new void SendEvent(MidiEvent midiEvent)
    {
        SendEventWithMetadata(midiEvent, null);
    }


    public override bool SendEventWithMetadata(MidiEvent midiEvent, object metadata)
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

    protected unsafe override bool SendMidiEvent(MidiEvent midiEvent, int? trackIndex)
    {
        //PluginLog.Information("Playing note");
        switch (midiEvent)
        {
            case ProgramChangeEvent @event:
                {
                    switch (Configuration.config.GuitarToneMode)
                    {
                        case GuitarToneMode.Off:
                            break;
                        case GuitarToneMode.Standard:
                            Channels[@event.Channel].Program = @event.ProgramNumber;

                            //int PCChannel = @event.Channel;
                            //SevenBitNumber currentProgram = Channels[PCChannel].Program;
                            //SevenBitNumber newProgram = @event.ProgramNumber;

                            //PluginLog.Verbose($"[N][ProgramChange][{trackIndex}:{@event.Channel}] {@event.ProgramNumber,-3} {@event.GetGMProgramName()}");

                            //if (currentProgram == newProgram) break;

                            //if (MidiBard.PlayingGuitar)
                            //{
                            //    uint instrument = MidiBard.ProgramInstruments[newProgram];
                            //    //if (!MidiBard.guitarGroup.Contains((byte)instrument))
                            //    //{
                            //    //    newProgram = MidiBard.Instruments[MidiBard.CurrentInstrument].ProgramNumber;
                            //    //    instrument = MidiBard.ProgramInstruments[newProgram];
                            //    //}

                            //    if (Channels[PCChannel].Program != newProgram)
                            //    {
                            //        PluginLog.Verbose($"[N][ProgramChange][{trackIndex}:{@event.Channel}] Changing guitar program to ({instrument} {MidiBard.Instruments[instrument].FFXIVDisplayName}) {@event.GetGMProgramName()}");
                            //    }
                            //}

                            //Channels[PCChannel].Program = newProgram;
                            break;
                        case GuitarToneMode.Simple:
                            Array.Fill(Channels, new ChannelState(@event.ProgramNumber));
                            break;
                        case GuitarToneMode.Override:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }


                    break;
                }
            case NoteOnEvent noteOnEvent:
                {
                    //PluginLog.Verbose($"[NoteOnEvent] [{trackIndex}:{noteOnEvent.Channel}] {noteOnEvent.NoteNumber,-3}");

                    var noteNum = GetTranslatedNoteNum(noteOnEvent.NoteNumber, trackIndex, out int octave);
                    var s = $"[N][DOWN][{trackIndex}:{noteOnEvent.Channel}] {GetNoteName(noteOnEvent)} ({noteNum})";

                    if (noteNum is < 0 or > 36)
                    {
                        s += "(out of range)";
                        //PluginLog.Verbose(s);
                        return false;
                    }

                    if (octave != 0) s += $"[adapted {octave:+#;-#;0} Oct]";

                    {
                        if (MidiBard.AgentPerformance.noteNumber - 39 == noteNum)
                        {
                            // release repeated note in order to press it again

                            if (playlib.ReleaseKey(noteNum))
                            {
                                MidiBard.AgentPerformance.Struct->PressingNoteNumber = -100;
                                // PluginLog.Verbose($"[N][PUP ][{trackIndex}:{noteOnEvent.Channel}] {GetNoteName(noteOnEvent)} ({noteNum})");
                            }
                        }

                        //PluginLog.Verbose(s);

                        if (playlib.PressKey(noteNum, ref MidiBard.AgentPerformance.Struct->NoteOffset,
                                ref MidiBard.AgentPerformance.Struct->OctaveOffset))
                        {
                            MidiBard.AgentPerformance.Struct->PressingNoteNumber = noteNum + 39;
                            return true;
                        }
                    }

                    break;
                }
            case NoteOffEvent noteOffEvent:
                {
                    var noteNum = GetTranslatedNoteNum(noteOffEvent.NoteNumber, trackIndex, out _);
                    if (noteNum is < 0 or > 36) return false;

                    if (MidiBard.AgentPerformance.Struct->PressingNoteNumber - 39 != noteNum)
                    {
#if DEBUG
                        //PluginLog.Verbose($"[N][IGOR][{trackIndex}:{noteOffEvent.Channel}] {GetNoteName(noteOffEvent)} ({noteNum})");
#endif
                        return false;
                    }

                    // only release a key when it been pressing
                    // PluginLog.Verbose($"[N][UP  ][{trackIndex}:{noteOffEvent.Channel}] {GetNoteName(noteOffEvent)} ({noteNum})");

                    if (playlib.ReleaseKey(noteNum))
                    {
                        MidiBard.AgentPerformance.Struct->PressingNoteNumber = -100;
                        return true;
                    }

                    break;
                }
        }

        return false;
    }


    public override int GetTranslatedNoteNum(int noteNumber, int? trackIndex, out int octave, bool plotting = false)
    {
        //PluginLog.Information("Playing note");
        noteNumber = noteNumber - 48;

        octave = 0;

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
}