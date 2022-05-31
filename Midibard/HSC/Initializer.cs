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
        private static async Task InitHSCoverride() {

            PluginLog.Information($"Using HSC override.");

            HSC.Settings.AppSettings.CurrentAppPath = DalamudApi.api.PluginInterface.AssemblyLocation.DirectoryName;

            await UpdateClientInfo();

            //InitIPC();
            CreateHSCPlaylistWatcher();

            //reload hsc playlist
            if (Configuration.config.useHscPlaylist)
                await Task.Run(() => {
                    HSCPlaylistHelpers.Reload();
                    HSCPlaylistHelpers.ReloadSettings();
                });
        }

        private static async Task UpdateClientInfo()
        {
            HSC.Settings.CharName = DalamudApi.api.ClientState.LocalPlayer?.Name.TextValue;
            HSC.Settings.CharIndex = await CharConfigHelpers.GetCharIndex(HSC.Settings.CharName);

            PluginLog.Information($"Updated HSC client info: index: {HSC.Settings.CharIndex}, char name: '{HSC.Settings.CharName}'.");
        }

    }
}
