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
        public static void ShiftTrack(TrackChunk trackChunk, int offset) => trackChunk.ProcessNotes(n => n.Time += offset, n => n.Time > 0);

        public static void Shift(MidiFile midiFile, MidiSequence settings)
        {
            var tracks = midiFile.GetTrackChunks();

            int index = 0;
            foreach (var track in tracks)
            {

    
                if (!settings.Tracks.ContainsKey(index))
                    continue;

                var trackSettings = settings.Tracks[index];

                if (trackSettings.TimeOffset != 0)
                {
                    PluginLog.Information($"Shifting track {index} by {trackSettings.TimeOffset}");
                    ShiftTrack(track, trackSettings.TimeOffset);
                }
            
                index++;
            }
        }

    }
}
