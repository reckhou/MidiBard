using Dalamud.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiBard.Common;
using MidiBard.HSC.Models;
using MidiBard.Control.MidiControl;
using MidiBard.Control.CharacterControl;
using System.Threading;

namespace MidiBard
{
    /// <summary>
    /// this will sync HSC playlist with midibard one
    /// </summary>
    public class HSCPlaylistHelpers
    {
        private static int currentPlaying;

        private static void UpdateTracks(string title, Dictionary<int, HSC.Music.Track> tracks)
        {

            PluginLog.Information($"Updating tracks for '{title}'");
            int index = 0;
            foreach (var track in tracks)
            {
                if (!track.Value.Muted && track.Value.EnsembleMember == HSC.Settings.CharIndex)
                {
                    PluginLog.Information($"Track {index} is assigned from HSC playlist");

                    //percussion + duplication logic. if track has parent enable its parent
                    if (track.Value.ParentIndex > -1)
                        ConfigurationPrivate.config.EnabledTracks[track.Value.ParentIndex] = true;
                    else//no parent enable as normal
                        ConfigurationPrivate.config.EnabledTracks[index] = true;
                }
                else
                    ConfigurationPrivate.config.EnabledTracks[index] = false;
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

        public static async Task ReloadSettings(bool loggedIn = false)
        {
            PluginLog.Information($"Reloading HSC playlist settings'");

            try
            {
                HSC.Settings.PlaylistSettings.Settings.Clear();

                await OpenPlaylistSettings();

                if (HSC.Settings.PlaylistSettings.Settings.IsNullOrEmpty())
                {
                    PluginLog.Information($"Reloading HSC playlist settings failed'");
                    return;
                }

                if (string.IsNullOrEmpty(Configuration.config.loadedMidiFile))
                    return;

                PluginLog.Information($"Switching instrument for '{Configuration.config.loadedMidiFile}'...");

                if (!loggedIn && !HSC.Settings.Playlist.Loaded)
                    await SwitchInstrument.WaitSwitchInstrumentForSong(Configuration.config.loadedMidiFile);

                var curItemSettings = HSC.Settings.PlaylistSettings.Settings[Configuration.config.loadedMidiFile];

                UpdateTracks(Configuration.config.loadedMidiFile, curItemSettings.Tracks);


                HSC.Settings.Playlist.Loaded = true;
                Thread.Sleep(5000);
                HSC.Settings.Playlist.Loaded = false;

            }

            catch (Exception e)
            {
                PluginLog.Error(e, $"Reloading HSC playlist failed. {e.Message}");
            }
        }

        public static async Task Reload(bool loggedIn = false)
        {
            PluginLog.Information($"Reloading HSC playlist'");

            try
            {
                bool wasPlaying = MidiBard.IsPlaying;

                if (wasPlaying)
                    currentPlaying = PlaylistManager.CurrentPlaying;

                HSC.Settings.Playlist.Clear();
                PlaylistManager.Clear();

                await OpenPlaylist();
                await OpenPlaylistSettings();

                if (HSC.Settings.Playlist.Files.IsNullOrEmpty())
                {
                    PluginLog.Information($"No songs in HSC playlist");
                    PerformActions.DoPerformAction(0);
                    PlaylistManager.CurrentPlaying = -1;
                    return;
                }

                PluginLog.Information($"Updating midibard playlist");
                await PlaylistManager.AddAsync(HSC.Settings.Playlist.Files.ToArray());
                PluginLog.Information($"Added {HSC.Settings.Playlist.Files.Count} files.");

                PluginLog.Information($"switching to {HSC.Settings.Playlist.SelectedIndex} from HSC playlist.");

                PlaylistManager.CurrentPlaying = currentPlaying;

                if (!wasPlaying && !loggedIn && !HSC.Settings.Playlist.Loaded)
                    MidiPlayerControl.SwitchSong(HSC.Settings.Playlist.SelectedIndex, false);

                var curItemSettings = HSC.Settings.PlaylistSettings.Settings[Configuration.config.loadedMidiFile];

                UpdateTracks(Configuration.config.loadedMidiFile, curItemSettings.Tracks);

                if (!loggedIn)
                    MidiBard.Ui.Open();

                HSC.Settings.Playlist.Loaded = true;
                Thread.Sleep(5000);
                HSC.Settings.Playlist.Loaded = false;

            }

            catch (Exception e)
            {
                PluginLog.Error(e, $"Reloading HSC playlist failed. {e.Message}");
            }
        }
    }
}
