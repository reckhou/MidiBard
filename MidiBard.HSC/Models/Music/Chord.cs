using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.MusicTheory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Music
{

    public class Chord
    {
        private Dictionary<int, Melanchall.DryWetMidi.Interaction.Note> notes;

        public Chord(long timeElapsed, IEnumerable<Melanchall.DryWetMidi.Interaction.Note> notes) : this(timeElapsed)
        {
            this.notes = new Dictionary<int, Melanchall.DryWetMidi.Interaction.Note>();
            this.AddNotes(notes);
        }

        public Chord(long timeElapsed)
        {
            this.Time = timeElapsed;
        }

        ~Chord()
        {
            this.notes = null;
        }

        public long Time { get; private set; }

        public Melanchall.DryWetMidi.Interaction.Note HighestNote => GetHighestNote();
        public Melanchall.DryWetMidi.Interaction.Note LowestNote => GetLowestNote();

        public Melanchall.DryWetMidi.Interaction.Note FirstNote => this.notes.Values.First();
        public Melanchall.DryWetMidi.Interaction.Note LastNote => this.notes.Values.Last();

        public Dictionary<int, Melanchall.DryWetMidi.Interaction.Note> Notes => notes;

        public int Length => this.notes.Count();

        public IEnumerable<int> TrackIndexes
        {
            get => this.Notes.Values.Select(n => n.TrackIndex).Distinct();
        }


        public override string ToString()
        {
            return GetNoteNames().CommaSeparated();
        }

        public IEnumerable<Melanchall.DryWetMidi.Interaction.Note> GetNotes(int trackIndex)
        {
            return this.Notes.Values.Where(n => n.TrackIndex == trackIndex);
        }

        public string[] GetNoteNames()
        {
            return this.notes.Values
                .OrderBy(ev => ev.NoteNumber)
                .Select(ev => NoteUtilities.GetNoteText(ev.NoteNumber))
                .ToArray();
        }

        public bool ContainsNote(Melanchall.DryWetMidi.Interaction.Note note)
        {
            return this.notes.Any(e => e.Value.NoteNumber == note.NoteNumber && e.Value.Time == note.Time);
        }

        public bool ContainsNoteValue(int value)
        {
            return this.notes.Any(e => e.Value.NoteNumber == value);
        }

        public void AddNotes(IEnumerable<Melanchall.DryWetMidi.Interaction.Note> notes)
        {
            foreach (var note in notes)
            {
                this.notes.AddIfNotKeyExists(note.NoteNumber, note);
            }

            this.notes = this.notes.Values
                .OrderBy(v => (int)v.NoteNumber)
                .ToDictionary(v => (int)v.NoteNumber, v => v);
        }

        public Music.Chord CombineWith(Music.Chord chord)
        {
            AddNotes(chord.Notes.Values);

            var newChord = new Music.Chord(Time, Notes.Values);

            return this;
        }

        public void AddNote(Melanchall.DryWetMidi.Interaction.Note noteEvent)
        {
                this.notes.AddIfNotKeyExists(noteEvent.NoteNumber, noteEvent);
        }

        public void ReplaceNotes(IEnumerable<Melanchall.DryWetMidi.Interaction.Note> noteEvents)
        {
            this.notes.Clear();
            this.AddNotes(noteEvents);
        }


        public IEnumerable<Melanchall.DryWetMidi.Interaction.Note> GetDifference(Music.Chord srcChord)
        {
            var notes = new List<Melanchall.DryWetMidi.Interaction.Note>();
            
            foreach(var note in this.Notes.Values)
            {
                if (!srcChord.ContainsNote(note))
                    notes.Add(note);
            }

            return notes;
        }

        private Melanchall.DryWetMidi.Interaction.Note GetHighestNote()
        {
            return this.notes.FirstOrDefault(n => n.Key == this.notes.Keys.Max()).Value;
        }

        private Melanchall.DryWetMidi.Interaction.Note GetLowestNote()
        {
            return this.notes.FirstOrDefault(n => n.Key == this.notes.Keys.Min()).Value;
        }
    }
}
