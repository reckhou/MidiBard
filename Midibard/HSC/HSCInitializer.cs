using Dalamud.Logging;
using MidiBard.HSC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard
{
    public partial class MidiBard
    {
        private static void InitHSCoverride() {

            PluginLog.Information($"Using HSC override.");

            HSC.Settings.AppSettings.CurrentAppPath = DalamudApi.api.PluginInterface.AssemblyLocation.DirectoryName;

            UpdateClientInfo();
            //InitIPC();
            CreateHSCPlaylistWatcher();

            if (Configuration.config.useHscPlaylist)
                Task.Run(() => {
                    HSCPlaylistHelpers.Reload();
                    HSCPlaylistHelpers.ReloadSettings();
                });
        }

        private static void UpdateClientInfo()
        {
            int procId = Process.GetCurrentProcess().Id;

            HSC.Settings.CharName = DalamudApi.api.ClientState.LocalPlayer?.Name.TextValue;
            HSC.Settings.CharIndex = GameProcessFinder.GetIndex(procId);

            PluginLog.Information($"Updated HSC client info: index: {HSC.Settings.CharIndex}, char name: '{HSC.Settings.CharName}'.");
        }

    }
}
}
