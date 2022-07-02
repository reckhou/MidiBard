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
using System.Diagnostics;

namespace MidiBard.HSC
{
    internal class MidiProcessor
    {
        public static TimedEventWithTrackChunkIndex[] Process(TimedEventWithTrackChunkIndex[] timedObjs, MidiSequence settings)
        {
            PluginLog.Information($"HSCM processing song '{Settings.AppSettings.CurrentSong}', {timedObjs.Count()} events before processing.");

            var stopwatch = Stopwatch.StartNew();


            var tracks = timedObjs.GroupBy(to => (int)to.Metadata)
                .Select(to => new { index = to.Key, track = TimedObjectUtilities.ToTrackChunk(to.ToArray()) })
                 .AsParallel().Where(c => c.track.GetNotes().Any())
              .OrderBy(t => t.index).ToDictionary(t => t.index, t => t.track);

            if (Configuration.config.useHscmChordTrimming)
                ChordTrimmer.Trim(tracks, settings, 2, false, Configuration.config.useHscmTrimByTrack);

            ProcessTracks(tracks, settings);

            var newEvs = tracks.Values
          .SelectMany((chunk, index) => chunk.GetTimedEvents().Select(e => new TimedEventWithTrackChunkIndex(e.Event, e.Time, index))).ToArray();

            PluginLog.Information($"HSCM process of '{Settings.AppSettings.CurrentSong}' finished in {stopwatch.Elapsed.TotalMilliseconds}, {newEvs.Count()} events after processing.");

            return newEvs;

        }

        public static void Process(MidiFile midiFile, MidiSequence settings)
        {
            PluginLog.Information($"HSCM processing song '{Settings.AppSettings.CurrentSong}', {midiFile.GetTimedEvents().Count()} events before processing.");

            var stopwatch = Stopwatch.StartNew();

            var tracks = midiFile.GetTrackChunks()
               .Select((t, i) => new { track = t, index = i })
              .AsParallel().Where(c => c.track.GetNotes().Any())
              .OrderBy(t => t.index).ToDictionary(t => t.index, t => t.track);

            if (Configuration.config.useHscmChordTrimming)
                ChordTrimmer.Trim(tracks, settings, 2, false, Configuration.config.useHscmTrimByTrack);

            PluginLog.Information($"Total notes in MIDI after trimming {midiFile.GetNotes().Count()}");

            ProcessTracks(tracks, settings);

            PluginLog.Information($"HSCM process of '{Settings.AppSettings.CurrentSong}' finished in {stopwatch.Elapsed.TotalMilliseconds}, {tracks.Select(t => t.Value).GetTimedEvents().Count()} events after processing.");
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
            if (trackSettings.TimeOffset != 0 && note.Time >= Math.Abs(trackSettings.TimeOffset))
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
            int newNote = 0;
            int oldNote = (int)note.NoteNumber;

            newNote = GetTransposedValue(oldNote, trackindex, settings);

            //PluginLog.Information($"old value: {oldNote}, new value: {newNote}");

            note.NoteNumber = new SevenBitNumber((byte)newNote);

        }

        private static int GetTransposedValue(int note, int trackIndex, MidiSequence settings)
        {
            int transposeVal = 0;

            //PluginLog.Information("Transposing from HSCM playlist");

            var trackInfo = GetHSCTrackInfo(trackIndex);

            if (trackInfo == null)
                return 0;
    
            if (trackInfo.OctaveOffset != 0)
                transposeVal += 12 * trackInfo.OctaveOffset;

            if (trackInfo.KeyOffset != 0)
                transposeVal += trackInfo.KeyOffset;

            //int newVal = (12 * settings.OctaveOffset) + settings.KeyOffset + transposeVal;

            return note+ transposeVal;
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
