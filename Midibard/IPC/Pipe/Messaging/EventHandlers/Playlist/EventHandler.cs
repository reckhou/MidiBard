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
    /// All playlist IPC command events go here and should follow similar approach
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="index"></param>
    private static async void PlaylistMessageHandler_ReloadMessageReceived(object sender, EventArgs e)
    {
        Dalamud.Logging.PluginLog.Information($"Reload playlist message received from IPC server.");

        await HSCPlaylistHelpers.Reload();
    }

    private static async void PlaylistMessageHandler_ReloadSongMessageReceived(object sender, string title)
    {
        Dalamud.Logging.PluginLog.Information($"Reload song '{title}' message received from IPC server.");

        await HSCPlaylistHelpers.LoadAndApplyHscPlaylistSettings(title);
    }
}
