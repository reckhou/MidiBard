using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSCM
{
    public class MidiPlayerControl
    {
        //loads song by name (selected from HSC) to prevent errors when HSC + MidiBard playlist not the same
        public static void SwitchSongByName(string name, bool startPlaying = false)
        {

            var song = MidiBardHSCMPlaylistManager.GetSongByName(name);

            if (song == null)
            {
                PluginLog.Error($"Error: song does not exist on playlist '{name}'.");
                return;
            }

            PlaylistManager.CurrentPlaying = song.Value.index;

            HSCMFilePlayback.LoadPlaybackFromSong(name, startPlaying, true, true, true);
        }
    }
}
