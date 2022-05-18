using MidiBard.Control.MidiControl;
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
    private static void PlaybackMessageHandler_PlayMessageReceived(object sender, int index)
    {
        MidiPlayerControl.SwitchSong(index);

        Dalamud.Logging.PluginLog.Debug($"Play '{index}' message received from IPC server.");
    }
}
