using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC
{
    internal class Chord
    {
        public Note LowestNote => Notes.MinElement(n => (int)n.NoteNumber);

        public Note HighestNote => Notes.MaxElement(n => (int)n.NoteNumber);

        public IEnumerable<Note> Notes { get; set; }

        public long Time { get; set; }

        public Chord()
        {

        }
    }
}
