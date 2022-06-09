using MidiBard.Control.MidiControl;
using MidiBard.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MidiBard;

public partial class MidiBard
{
    /// <summary>
    /// All playback IPC command events go here and should follow similar approach
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="index"></param>
    private static void PlaybackMessageHandler_PlayMessageReceived(object sender, string title)
    {
        Dalamud.Logging.PluginLog.Information($"Play '{title}' message received from IPC server.");

        int index = Configuration.config.Playlist.GetIndex(title);
        if (index > -1)
            MidiPlayerControl.SwitchSong(index, true);
    }
}
