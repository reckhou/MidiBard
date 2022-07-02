using Dalamud.Logging;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using MidiBard.Control;
using MidiBard.Control.MidiControl;
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
        public static TimedEventWithTrackChunkIndex[] GetTimedEvents (MidiFile midiFile)
        {
            var timedEvents = midiFile.GetTrackChunks()
                      .SelectMany((chunk, index) => chunk.GetTimedEvents().Select(e =>
                      {
                          var compareValue = e.Event switch
                          {
                    //order chords so they always play from low to high
                              NoteOnEvent noteOn => noteOn.NoteNumber,
                    //order program change events so they always get processed before notes 
                              ProgramChangeEvent => -2,
                    //keep other unimportant events order
                              _ => -1
                          };
                          return (compareValue, timedEvent: new TimedEventWithTrackChunkIndex(e.Event, e.Time, index));
                      }))
                      .OrderBy(e => e.timedEvent.Time)
                      .ThenBy(i => i.compareValue)
                      .Select(i => i.timedEvent).ToArray(); //this is crucial as have executed a parallel query

            return timedEvents.Select(ev => ev.Copy()).ToArray();
        }

        public static Playback GetProcessedMidiPlayback(MidiFile midiFile, string songName)
        {
            var settings = HSC.Settings.PlaylistSettings.Settings[songName];

            MidiProcessor.Process(midiFile, settings);

            return FilePlayback.GetFilePlayback(midiFile, songName);
        }

        private static BardPlayback GetPlayback(TimedEventWithTrackChunkIndex[] timedEvs, TempoMap tempoMap, string songName)
        {
            var playbackInfo = FilePlayback.GetPlayback(timedEvs, tempoMap, songName);

            return playbackInfo.playback;
        }

        public static BardPlayback GetProcessedPlayback(TimedEventWithTrackChunkIndex[] timedEvs, TempoMap tempoMap, string songName)
        {
            var settings = HSC.Settings.PlaylistSettings.Settings[songName];
 
            timedEvs = MidiProcessor.Process(timedEvs, settings);

            return GetPlayback(timedEvs, tempoMap, songName);
        }

        public static BardPlayback GetCachedPlayback(string songName)
        {
            if (HSC.SongCache.IsCached(songName))
            {
                PluginLog.Information($"Fetching '{songName}' from HSCM cache");

                var cacheItem = SongCache.Item(songName);

                if (!cacheItem.HasValue)
                    return null;

                var item = cacheItem.Value as (TempoMap tempoMap, TimedEventWithTrackChunkIndex[] timedEvents)?;
                var evs = item.Value.timedEvents.Select(ev => ev.Copy()).ToArray();
                PluginLog.Information($"Loading song {songName} from cache, {evs.Count()} events");


                return GetProcessedPlayback(evs, item.Value.tempoMap, songName);
            }
            return null;
        }

    }
}
