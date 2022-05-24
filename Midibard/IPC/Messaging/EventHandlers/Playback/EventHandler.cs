using MidiBard.Control.MidiControl;
using MidiBard.Util;
using System;
using System.Collections.Generic;
using System.IO;
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
        int index = Configuration.config.Playlist
            .Select(p => Path.GetFileNameWithoutExtension(p).ToLower()).ToList().GetIndex(title.ToLower());

        //foreach(var item in Configuration.config.Playlist)
        //    Dalamud.Logging.PluginLog.Information(Path.GetFileNameWithoutExtension(item));

        Dalamud.Logging.PluginLog.Information($"Play '{title}' ({index}) message received from IPC server.");

        if (index > -1)
            MidiPlayerControl.SwitchSong(index, true);
    }
}
