using Dalamud.Logging;
using MidiBard.HSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard
{
    public partial class MidiBard
    {


        private static void MsgHandler_ChangeSongMessageReceived(object sender, int index)
        {
            PluginLog.Information($"Received change song '{index}' message.");
            Task.Run(() => HSCMPlaylistManager.ChangeSong(index));
        }

        private static void MsgHandler_ReloadPlaylistSettingsMessageReceived(object sender, EventArgs e)
        {
            PluginLog.Information($"Received reload playlist settings message.");
            Task.Run(() => HSCMPlaylistManager.ReloadSettingsAndSwitch());
        }

        private static void MsgHandler_ReloadPlaylistMessageReceived(object sender, EventArgs e)
        {
            PluginLog.Information($"Received reload playlist message.");
            Task.Run(() => HSCMPlaylistManager.Reload());
        }

        private static void MsgHandler_SwitchInstrumentsMessageReceived(object sender, EventArgs e)
        {
            PluginLog.Information($"Received switch instruments message.");
            Task.Run(() => HSCMPlaylistManager.SwitchInstruments());
        }

        private static void MsgHandler_RestartHscmOverrideMessageReceived(object sender, EventArgs e)
        {
            PluginLog.Information($"Received restart HSCM override message.");
            Task.Run(() => RestartHSCMOverride());
        }

        private static void MsgHandler_ClosePerformanceMessageReceived(object sender, EventArgs e)
        {
            PluginLog.Information($"Received close performance message.");
            Task.Run(() => PerformHelpers.ClosePerformance());
        }
    }
}
