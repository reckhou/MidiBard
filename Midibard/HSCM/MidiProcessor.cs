using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using MidiBard.HSC.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiBard.Common;
using MidiBard.Control.MidiControl.PlaybackInstance;
using Dalamud.Logging;

namespace MidiBard.HSC
{
    internal class MidiProcessor
    {
        public static TimedEventWithTrackChunkIndex[] Process(TimedEventWithTrackChunkIndex[] timedObjs, MidiSequence settings)
        {
            PluginLog.Information($"Processing song '{Settings.AppSettings.CurrentSong}'");

            var tracks = timedObjs.GroupBy(to => (int)to.Metadata)
                .Select(to => new { index = to.Key, track = TimedObjectUtilities.ToTrackChunk(to.ToArray()) })
                 .AsParallel().Where(c => c.track.GetNotes().Any())
              .OrderBy(t => t.index).ToDictionary(t => t.index, t => t.track);

            if (Configuration.config.useHscmChordTrimming)
                ChordTrimmer.Trim(tracks, settings, 2, false, Configuration.config.useHscmTrimByTrack);

            ProcessTracks(tracks, settings);

            return tracks.Values
          .SelectMany((chunk, index) => chunk.GetTimedEvents().Select(e => new TimedEventWithTrackChunkIndex(e.Event, e.Time, index))).ToArray();

        }

        public static void Process(MidiFile midiFile, MidiSequence settings)
        { 
            var tracks = midiFile.GetTrackChunks()
               .Select((t, i) => new { track = t, index = i })
              .AsParallel().Where(c => c.track.GetNotes().Any())
              .OrderBy(t => t.index).ToDictionary(t => t.index, t => t.track);

            if (Configuration.config.useHscmChordTrimming)
                ChordTrimmer.Trim(tracks, settings, 2, false, Configuration.config.useHscmTrimByTrack);

            PluginLog.Information($"Total notes in MIDI after trimming {midiFile.GetNotes().Count()}");

            ProcessTracks(tracks, settings);
        }

        private static void ProcessTracks(Dictionary<int, TrackChunk> tracks, MidiSequence settings)
        {
            Parallel.ForEach(tracks, t =>
            {
                if (settings.Tracks.ContainsKey(t.Key))
                {
                    var trackSettings = settings.Tracks[t.Key];

                    t.Value.ProcessNotes(n => ProcessNote(n, trackSettings, t.Key, settings));
                    t.Value.RemoveNotes(n => !ShouldPlayDrumNote(n.NoteNumber, t.Key)); //remove the drum notes this person should not play
                }
            });
        }

        private static void ProcessNote(Note note, Track trackSettings, int trackIndex, MidiSequence settings)
        {
            if (trackSettings.TimeOffset != 0)
                ShiftTime(note, trackSettings.TimeOffset);

            try
            {
                if (Configuration.config.useHscmTransposing)
                Transpose(note, trackIndex,  settings);
            }
            catch { }
        }

        private static void ShiftTime(Note note, int offset) => note.Time += offset;

        private static void Transpose(Note note, int trackindex, MidiSequence settings)
        {
            int newVal = GetTransposedValue(trackindex, settings);
            PluginLog.Information($"new value: {newVal}");
            if (newVal == 0)
                return;

            if (newVal < 0)
                note.NoteNumber = new SevenBitNumber((byte)((byte)note.NoteNumber-newVal));
            else
                note.NoteNumber = new SevenBitNumber((byte)((byte)note.NoteNumber+newVal));

        }

        private static int GetTransposedValue(int trackIndex, MidiSequence settings)
        {
            //PluginLog.Information("Transposing from HSCM playlist");

            int noteNum = 0;

            var trackInfo = GetHSCTrackInfo(trackIndex);

            if (trackInfo == null)
                return 0;
    
            if (trackInfo.OctaveOffset != 0)
                noteNum += 12 * trackInfo.OctaveOffset;

            if (trackInfo.KeyOffset != 0)
                noteNum += trackInfo.KeyOffset;

            //int newVal = 12 * settings.OctaveOffset + settings.KeyOffset + noteNum;

            return noteNum;
        }

        private static Settings.TrackTransposeInfo GetHSCTrackInfo(int trackIndex)
        {
            if (Settings.MappedTracks.ContainsKey(trackIndex))
                return Settings.MappedTracks[trackIndex];
            
            if (!Settings.TrackInfo.ContainsKey(trackIndex))
                return null;

            return Settings.TrackInfo[trackIndex];
        }
        
        private static bool ShouldPlayDrumNote(int noteNum, int trackIndex)
        {
            if (HSC.Settings.PercussionNotes.IsNullOrEmpty())
                return true;

            //not a percussion note so play anyway
            if (!HSC.Settings.PercussionNotes.ContainsKey(trackIndex))
                return true;

            //percussion note - do percussion logic
            return HSC.Settings.PercussionNotes[trackIndex].ContainsKey(noteNum) && HSC.Settings.PercussionNotes[trackIndex][noteNum];
        }

    }
}
