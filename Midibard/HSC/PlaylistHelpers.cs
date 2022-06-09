using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiBard.Common;
using MidiBard.HSC.Models;

namespace MidiBard
{
    /// <summary>
    /// this will sync HSC playlist with midibard one
    /// </summary>
    public class HSCPlaylistHelpers
    {

        private static void UpdateTracks(string title, Dictionary<int, HSC.Music.Track> tracks)
        {
            PluginLog.Information($"Updating tracks for '{title}'");

            int index = 0;
            foreach (var track in tracks)
            {
                if (!track.Value.Muted && track.Value.EnsembleMember == HSC.Settings.CharIndex)
                {
                    PluginLog.Information($"Track {index} is assigned from HSC playlist");
                    ConfigurationPrivate.config.EnabledTracks[index] = true;
                }
                if (track.Value.Muted || track.Value.EnsembleMember != HSC.Settings.CharIndex)
                {
                    ConfigurationPrivate.config.EnabledTracks[index] = false;
                }
                index++;
            }
        }

        private static async Task OpenPlaylist()
        {
            string path = Path.Join(HSC.Settings.AppSettings.CurrentAppPath, Configuration.config.hscPlayListPath);

            var files = Directory.GetFiles(path, "*.pl");

            if (files.IsNullOrEmpty())
            {

                PluginLog.Information($"No HSC playlists found.'");
                return;
            }

            var playlistFile = files.First();

            PluginLog.Information($"HSC playlist path: '{playlistFile}'");

            await HSC.Playlist.Playlist.OpenPlaylist(playlistFile, false);

            PluginLog.Information($"Load HSC playlist '{playlistFile}' success. total songs: {HSC.Settings.Playlist.Files.Count}");
        }

        private static async Task OpenPlaylistSettings()
        {
            string path = Path.Join(HSC.Settings.AppSettings.CurrentAppPath, Configuration.config.hscPlayListPath);

            var files = Directory.GetFiles(path, "*.json");

            if (files.IsNullOrEmpty())
            {

                PluginLog.Information($"No HSC playlist settings found.'");
                return;
            }

            var settingsFile = files.First();

            PluginLog.Information($"HSC playlist settings path: '{settingsFile}'");

            await HSC.Playlist.Playlist.LoadPlaylistSettings(settingsFile);

            PluginLog.Information($"Load HSC playlist '{settingsFile}' success. total songs: {HSC.Settings.Playlist.Files.Count}");
        }

        public static async Task ReloadSettings()
        {
            PluginLog.Information($"Reloading HSC playlist settings'");

            try
            {
                HSC.Settings.PlaylistSettings.Settings.Clear();

                await OpenPlaylistSettings();

                if (HSC.Settings.PlaylistSettings.Settings.IsNullOrEmpty())
                    return;

                PluginLog.Information($"Updating tracks for {HSC.Settings.PlaylistSettings.Settings.Count} files");

                foreach (var setting in HSC.Settings.PlaylistSettings.Settings)
                    UpdateTracks(setting.Key, setting.Value.Tracks);

            }

            catch (Exception e)
            {
                PluginLog.Error(e, $"Reloading HSC playlist failed. {e.Message}");
            }
        }

        public static async Task Reload()
        {
            PluginLog.Information($"Reloading HSC playlist'");

            try
            {
                HSC.Settings.Playlist.Clear();
                PlaylistManager.Clear();

                await OpenPlaylist();

                if (HSC.Settings.Playlist.Files.IsNullOrEmpty())
                    return;

                PluginLog.Information($"Updating midibard playlist");

               PlaylistManager.Add(HSC.Settings.Playlist.Files.ToArray());

                PluginLog.Information($"Added {HSC.Settings.Playlist.Files.Count} files.");

            }

            catch (Exception e)
            {
                PluginLog.Error(e, $"Reloading HSC playlist failed. {e.Message}");
            }
        }

        public static async Task LoadAndApplyHscPlaylistSettings(string fileName)
        {
            PluginLog.Information($"Loading HSC playlist settings for '{fileName}'");

            try
            {

                await OpenPlaylistSettings();

                var curItemSettings = HSC.Settings.PlaylistSettings.Settings[fileName];

                PluginLog.Information($"Load HSC playlist settings for '{fileName}' success.");

                PluginLog.Information($"Total tracks: '{curItemSettings.Tracks.Count}'");

                UpdateTracks(fileName, curItemSettings.Tracks);

            }

            catch (Exception e)
            {
                PluginLog.Error(e, $"Loading HSC playlist failed. {e.Message}");
            }
        }
    }
}
