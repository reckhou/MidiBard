
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace MidiBard.HSC.Music
{
    /// <summary>
    /// wrapper for a MIDI sequence
    /// </summary>

    [JsonObject(MemberSerialization.OptIn)]
    public class MidiSequence : IDisposable
    {
        private const int DefaultBpm = 120;

        public MidiSequence(MidiSequence midiSequence)
        {
            Info = midiSequence.Info;
            Tracks = midiSequence.Tracks.ToDictionary(x => x.Key, x => x.Value);

            PlayAll = midiSequence.PlayAll;

            ReduceMaxNotes = midiSequence.ReduceMaxNotes;
            ReduceType = midiSequence.ReduceType;

            KeyOffset = midiSequence.KeyOffset;
            OctaveOffset = midiSequence.OctaveOffset;

            HoldLongNotes = midiSequence.HoldLongNotes;
        }

        public MidiSequence()
        {
            Info = new SequenceInfo();
            Tracks = new Dictionary<int, Music.Track>();

            PlayAll = true;

            ReduceMaxNotes = 2;
            ReduceType = 1;

            KeyOffset = 0;
            OctaveOffset = 0;
        }

        public MidiSequence(string filePath) : this()
        {
            Info = new SequenceInfo(filePath);
        }

        ~MidiSequence()
        {
            Cleanup();
        }

        #region properties
        [JsonProperty]
        public int Index { get; set; }

        [JsonProperty]
        public int MinLength { get; set; }


        [JsonProperty]
        public bool HoldLongNotes { get; set; }

        [JsonProperty]
        public Dictionary<int, Music.Track> Tracks { get; set; }

        public SequenceInfo Info { get; private set; }



        [JsonProperty]
        public int KeyOffset { get; set; }

        [JsonProperty]
        public int OctaveOffset { get; set; }

        [JsonProperty]
        public int ReduceMaxNotes { get; set; }

        [JsonProperty]
        public int ReduceType { get; set; }

        [JsonProperty]
        public bool HighestOnly { get; set; }

        [JsonProperty]
        public bool PlayAll { get; set; }

        [JsonProperty]
        public string Instrument { get;  set; }
        #endregion

        #region public methods


        public bool TrackIsMuted(int index)
        {
            return this.Tracks.ContainsKey(index) && this.Tracks[index].Muted;
        }

        public void Dispose()
        {
            Cleanup();
        }
        #endregion


        #region private methods
        private void Cleanup()
        {
            this.Tracks = null;
        }
        #endregion

    }
}
