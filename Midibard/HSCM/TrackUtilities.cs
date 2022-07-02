using Dalamud.Logging;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using MidiBard.HSC.Music;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC
{
    public class TrackUtilities
    {
        public static void ShiftTrack(TrackChunk trackChunk, int offset) => trackChunk.ProcessNotes(n => n.Time += offset, n => n.Time >= offset);


        public static void Transpose(TrackChunk trackChunk, int offset) => trackChunk.ProcessNotes(n => n.NoteNumber = new Melanchall.DryWetMidi.Common.SevenBitNumber((byte)((byte)n.NoteNumber + offset)));

        public static void Shift(Dictionary<int, TrackChunk> tracks, MidiSequence settings)
        {
           Parallel.ForEach(tracks, t =>
            {
                if (settings.Tracks.ContainsKey(t.Key))
                {
                    var trackSettings = settings.Tracks[t.Key];

                    if (trackSettings.TimeOffset != 0)
                    {
                        PluginLog.Information($"Shifting track {t.Key} by {trackSettings.TimeOffset}");
                        ShiftTrack(t.Value, trackSettings.TimeOffset);
                    }
                }
            });
        }
    }
}
