using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using MidiBard.Control.MidiControl.PlaybackInstance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC
{
    /// <summary>
    /// experimental and for performance improvements - we dont need to reload the MIDI if already loaded
    /// will only exist during lifetime of the plugin, but later can serialize/deserialize to binary if need be to prevent further midi processing as an option
    /// </summary>
    internal static class SongCache
    {
        static SongCache()
        {
            songs = new Dictionary<string, (TempoMap tempoMap, TimedEventWithTrackChunkIndex[] timeEvents)>();
        }

        private static Dictionary<string, (TempoMap tempoMap, TimedEventWithTrackChunkIndex[] timeEvents)> songs { get; set; } 

        public static bool IsCached(string songName) => songs.ContainsKey(songName);

        public static (TempoMap, TimedEventWithTrackChunkIndex[])? Item(string songName)
        {

            if (!songs.ContainsKey(songName))
                return null;

            var song = songs[songName];

            return (song.tempoMap, song.timeEvents.ToArray());
        }

        public static void AddOrUpdate(string songName, (TempoMap, TimedEventWithTrackChunkIndex[]) song)
        {
            if (!songs.ContainsKey(songName))
                songs.Add(songName, song);
            else
                songs[songName] = song;
        }
    }
}
