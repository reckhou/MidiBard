

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using MidiBard.HSC.Helpers;

namespace MidiBard.HSC.Music
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Track
    {
        private string ensembleInstrument;

        public Track Clone()
        {
            var track = new Track();

            track.OctaveOffset = OctaveOffset;
            track.KeyOffset = KeyOffset;
            track.Title = Title;
            track.Instrument =Instrument;
            track.Index = Index;
            track.PrevSelectedMember = PrevSelectedMember;
            track.EnsembleMember = EnsembleMember;
            track.EnsembleInstrument = EnsembleInstrument;
            track.PlayAll = PlayAll;
            track.ReduceMaxNotes = ReduceMaxNotes;
            track.HoldLongNotes = HoldLongNotes;
            track.Muted = Muted;

            track.HighestChordSize = HighestChordSize;
            track.TotalChords = TotalChords;
            track.TotalNotes = TotalNotes;
            track.Range = Range;

            return track;
        }

        public Track()
        {
            this.OctaveOffset = 0;
            this.KeyOffset = 0;
            this.PrevSelectedMember = 0;
            this.EnsembleMember = 0;
            this.EnsembleInstrument = "None";
            this.PlayAll = true;
            this.ReduceMaxNotes = 2;
            HoldLongNotes = false;
        }

        public Track(string name, string instrument, int index) : base()
        {


            this.OctaveOffset = 0;
            this.KeyOffset = 0;
            this.PrevSelectedMember = 0;
            this.EnsembleMember = 0;
            this.EnsembleInstrument = "None";
            this.PlayAll = true;
            this.ReduceMaxNotes = 2;
            HoldLongNotes = false;

            this.Title = name;
            this.Instrument = instrument;
            this.Index = index;

        }

        [JsonProperty]
        public int ParentIndex { get; set; }

        public int HighestNote { get; set; }

        public int Index { get; set;  }

        [JsonProperty]
        public int? PrevSelectedMember { get; set; }

        [JsonProperty]
        public int OctaveOffset { get; set; }

        [JsonProperty]
        public int TimeOffset { get; set; }

        [JsonProperty]
        public int KeyOffset { get; set; }

        public string Title { get; set; }

        public string Instrument { get; set; }

        [JsonProperty]
        public bool Muted { get; set; }

        public bool HasChanged { get; set; }

        [JsonProperty]
        public int? EnsembleMember { get; set; }

        [JsonProperty]
        public string EnsembleInstrument { get; set; }


        [JsonProperty]
        public bool HoldLongNotes { get; set; }

        [JsonProperty]
        public int ReduceMaxNotes { get; set; }

        [JsonProperty]
        public bool HighestOnly { get; set; }

        [JsonProperty]
        public bool PlayAll { get; set; }

        public int HighestChordSize { get; set; }

        public int TotalChords { get; set; }

        public int TotalNotes { get; set; }

        public string Range { get; set; }

        public void Reset()
        {
            PlayAll = true;
            HighestOnly = false;
            HoldLongNotes = false;
            OctaveOffset = 0;
            KeyOffset = 0;
            Muted = false;
            ReduceMaxNotes = 2;
        }
    }
}
