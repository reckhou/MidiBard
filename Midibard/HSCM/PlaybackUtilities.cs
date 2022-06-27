using Dalamud.Logging;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using MidiBard.Control.MidiControl.PlaybackInstance;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC
{
    internal class PlaybackUtilities
    {
        public static BardPlayback GetCachedPlayback(string songName)
        {
            if (HSC.SongCache.IsCached(songName))
            {
                var cacheItem = SongCache.Item(songName);

                if (!cacheItem.HasValue)
                    return null;

                var item = cacheItem.Value as (TempoMap tempoMap, TimedEventWithTrackChunkIndex[] timedEvents)?;

                MidiProcessor.Process(item.Value.timedEvents.ToArray(), HSC.Settings.CurrentSongSettings);

                return new BardPlayback(item.Value.timedEvents, item.Value.tempoMap, new MidiClockSettings { CreateTickGeneratorCallback = () => new HighPrecisionTickGenerator() });
            }
            return null;
        }

    }
}
