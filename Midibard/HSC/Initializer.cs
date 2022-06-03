using Dalamud.Logging;
using MidiBard.HSC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MidiBard
{
    public partial class MidiBard
    {

        private async void HSCCleanup()
        {
            //free watchers
            if (charConfigWatcher != null)
            {
                charConfigWatcher.Stop();
                charConfigWatcher.Dispose();
                charConfigWatcher = null;
            }

            if (hscPlaylistWatcher != null)
            {
                hscPlaylistWatcher.Stop();
                hscPlaylistWatcher.Dispose();
                hscPlaylistWatcher = null;
            }

            HSC.Settings.CharName = null;
            HSC.Settings.CharIndex = -1;

            HSC.Settings.Playlist.Clear();
            HSC.Settings.PlaylistSettings.Clear();
        }

        private static void InitHSCoverride(bool wait = false) {

            PluginLog.Information($"Using HSC override.");

            HSC.Settings.AppSettings.CurrentAppPath = DalamudApi.api.PluginInterface.AssemblyLocation.DirectoryName;

            if (wait)//wait until fully logged in
                Thread.Sleep(Configuration.config.HscOverrideDelay);

            UpdateClientInfo();

            //InitIPC();
            CreateHSCPlaylistWatcher();
            CreateCharConfigWatcher();

            //reload hsc playlist
            if (Configuration.config.useHscPlaylist)
            {
                HSCPlaylistHelpers.Reload(true);
                HSCPlaylistHelpers.ReloadSettings(true);
            }
        }

        private static async Task UpdateClientInfo()
        {
            HSC.Settings.CharName = DalamudApi.api.ClientState.LocalPlayer?.Name.TextValue;
            HSC.Settings.CharIndex = await CharConfigHelpers.GetCharIndex(HSC.Settings.CharName);

            PluginLog.Information($"Client logged in. HSC client info - index: {HSC.Settings.CharIndex}, character name: '{HSC.Settings.CharName}'.");
        }

    }
}
