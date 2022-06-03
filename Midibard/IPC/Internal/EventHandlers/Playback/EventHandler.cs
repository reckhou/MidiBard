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
    /// All playback internal IPC command events go here and should follow similar approach
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="index"></param>
    private static void PlaybackMessageHandler_TempoChangeMessageReceived(object sender, float speed)
    {
        Dalamud.Logging.PluginLog.Information($"Speed change '{speed}' message received from internal IPC.");
        MidiPlayerControl.SetSpeed(speed);
    }

    private static void PlaybackMessageHandler_TimeChangeMessageReceived(object sender, int time)
    {
        Dalamud.Logging.PluginLog.Information($"Time change '{time}' message received internal IPC.");
        MidiPlayerControl.ChangeDeltaTime(time);
    }
}
