using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using MidiBard.HSC.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiBard.Common;

namespace MidiBard.HSC
{
    internal class ChordTrimmer
    {
        public static void Trim(
            Dictionary<int, TrackChunk> tracks,
            MidiSequence settings, 
            int maxNotes = 2, 
            bool ignoreSettings = false, 
            bool perTrack = false)
        {
            if (perTrack)
            {
                Parallel.ForEach(tracks, t =>
                {
                    if (settings.Tracks.ContainsKey(t.Key))
                    {
                        var trackSettings = settings.Tracks[t.Key];

                        TrimTrack(t.Value, trackSettings, maxNotes, ignoreSettings);
                    }
                });

            }
            else
                TrimFile(tracks, settings, maxNotes, ignoreSettings);
        }


        private static void TrimFile(Dictionary<int, TrackChunk> tracks, MidiSequence settings, int maxNotes = 2, bool ignoreSettings = false)
        {
            var trackChunks = tracks.Select(t => t.Value);

            var chords = GetChords(trackChunks.GetNotes());

            trackChunks.RemoveNotes(n => chords.Any(c => c.Time == n.Time && ShouldRemoveNote(
                    c.Notes.ToArray(),
                    c.LowestNote, 
                    c.HighestNote, 
                    n, 
                    settings, 
                    maxNotes,
                    ignoreSettings)));
        }

        private static void TrimTrack(TrackChunk chunk, Track trackSettings, int maxNotes = 2, bool ignoreSettings = false)
        {
            var chords = GetChords(chunk.GetNotes());

            chunk.RemoveNotes(n => chords.Any(c => c.Time == n.Time && ShouldRemoveNote(
                    c.Notes.ToArray(),
                    c.LowestNote,
                    c.HighestNote,
                    n,
                    trackSettings,
                    maxNotes,
                    ignoreSettings)));
        }

        private static bool ShouldRemoveNote(
    Note[] chordNotes,
    Note lowest,
    Note highest,
    Note note,
    MidiSequence settings,
    int max = 2,
    bool ignoreSettings = false)
        {
            if (!ignoreSettings)
            {
                if (settings.PlayAll)
                    return false;
                if (settings.HighestOnly && note.NoteNumber < highest.NoteNumber)
                    return true;
                if (settings.ReduceMaxNotes == 2 && note.NoteNumber > lowest.NoteNumber && note.NoteNumber < highest.NoteNumber)
                    return true;
                return !GetNoteRange(chordNotes, lowest, highest, settings.ReduceMaxNotes).Any(no => no.NoteNumber == note.NoteNumber);
            }

            return ShouldRemoveNote(chordNotes, lowest, highest, note, max);
        }

        private static bool ShouldRemoveNote(
            Note[] chordNotes, 
            Note lowest, 
            Note highest,
            Note note, 
            Track trackSettings,
            int max = 2,
            bool ignoreSettings = false)
        {
            if (!ignoreSettings)
            {
                if (trackSettings.PlayAll)
                    return false;
                if (trackSettings.HighestOnly && note.NoteNumber < highest.NoteNumber)
                    return true;
                if (trackSettings.ReduceMaxNotes == 2 && note.NoteNumber > lowest.NoteNumber && note.NoteNumber < highest.NoteNumber)
                    return true;
                return !GetNoteRange(chordNotes, lowest, highest, trackSettings.ReduceMaxNotes).Any(no => no.NoteNumber == note.NoteNumber);
            }
            return ShouldRemoveNote(chordNotes, lowest, highest, note, max);
        }

        private static bool ShouldRemoveNote(
            Note[] notes, 
            Note lowest, 
            Note highest, 
            Note note, 
            int max = 2)
        {
            if (max == 1)
                return note.NoteNumber < highest.NoteNumber;
            if (max == 2)
                return note.NoteNumber > lowest.NoteNumber && note.NoteNumber < highest.NoteNumber;

            return !GetNoteRange(notes, lowest, highest, max).Any(no => no.NoteNumber == note.NoteNumber);
        }

        private static Note[] GetNoteRange(Note[] chordNotes, Note lowest, Note highest, int max = 2)
        {

            var notes = new[] { lowest }
                .Concat(chordNotes.Skip(1).Take(max - 2))
                .Concat(new[] { highest });

            return notes.ToArray();
        }

        private static IEnumerable<Chord> GetChords(IEnumerable<Note> notes)
        {
                var groupsByTime = notes.DictionaryGroupBy(n => n.Time, n => n.Value.Count() > 1);

                var chords = groupsByTime.Select(grp => new Chord() { Time = grp.Key, Notes = grp.Value.OrderBy(n => n.NoteNumber) });

                return chords;
        }
    }
}
