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
using MidiBard.HSC.Music;
using static MidiBard.HSC.Settings;

namespace MidiBard
{
    /// <summary>
    /// this will sync HSC playlist with midibard one
    /// </summary>
    public class HSCPlaylistHelpers
    {
        private static int currentPlaying;
        private static bool wasPlaying;

        private static void UpdatePercussionNote(int trackIndex, int note)
        {
            if (!HSC.Settings.PercussionNotes.ContainsKey(trackIndex))
                HSC.Settings.PercussionNotes[trackIndex] = new Dictionary<int, bool>() { { note, true } };
            else
                HSC.Settings.PercussionNotes[trackIndex].Add(note, true);
        }


        private static void UpdateMappedTracks(int parentIndex, TrackTransposeInfo info)
        {
            if (!HSC.Settings.MappedTracks.ContainsKey(parentIndex))
                HSC.Settings.MappedTracks.Add(parentIndex, info);
        }

        private static void UpdateTracks(string title, MidiSequence seq)
        {

            PluginLog.Information($"Updating tracks for '{title}'");

            int index = 0;

            HSC.Settings.OctaveOffset = seq.OctaveOffset;
            HSC.Settings.KeyOffset = seq.KeyOffset;

            HSC.Settings.PercussionNotes = new Dictionary<int, Dictionary<int, bool>>();
            HSC.Settings.MappedTracks = new Dictionary<int, TrackTransposeInfo>(); 
            HSC.Settings.TrackInfo = new Dictionary<int, TrackTransposeInfo>();

            foreach (var track in seq.Tracks)
            {
                var info = new TrackTransposeInfo() { KeyOffset = track.Value.KeyOffset, OctaveOffset = track.Value.KeyOffset };

                HSC.Settings.TrackInfo.Add(index, info);

                if (!track.Value.Muted && track.Value.EnsembleMember == HSC.Settings.CharIndex)
                {
                    if (track.Value.PercussionNote.HasValue)
                    {
                        PluginLog.Information($"Percussion track {index} ({track.Value.PercussionNote.Value}) has parent {track.Value.ParentIndex} from HSC playlist");
                        UpdatePercussionNote(track.Value.ParentIndex.Value, track.Value.PercussionNote.Value);
                    }

                    PluginLog.Information($"Track {index} is assigned from HSC playlist");

                    //percussion + duplication logic. if track has parent enable its parent
                    if (track.Value.ParentIndex.HasValue)
                    {
                        ConfigurationPrivate.config.EnabledTracks[track.Value.ParentIndex.Value] = true;

                            UpdateMappedTracks(track.Value.ParentIndex.Value, info);
                    }
                    else//no parent enable as normal
                        ConfigurationPrivate.config.EnabledTracks[index] = true;
                }
                else
                    ConfigurationPrivate.config.EnabledTracks[index] = false;
                index++;
            }

            //foreach (var pn in HSC.Settings.PercussionNotes)
            //{
            //    foreach (var n in pn.Value)
            //    {
            //        PluginLog.Information($"Percussion Note {pn.Key} {n.Key}");
            //    }
            //}
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

                if (!loggedIn && !wasPlaying && !HSC.Settings.Playlist.Loaded)
                    await SwitchInstrument.WaitSwitchInstrumentForSong(Configuration.config.loadedMidiFile);

                var curItemSettings = HSC.Settings.PlaylistSettings.Settings[Configuration.config.loadedMidiFile];

                UpdateTracks(Configuration.config.loadedMidiFile, curItemSettings);


                HSC.Settings.Playlist.Loaded = true;
                Thread.Sleep(2000);
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
                wasPlaying = MidiBard.IsPlaying;


                HSC.Settings.Playlist.Clear();
                PlaylistManager.Clear();

                await OpenPlaylist();
                await OpenPlaylistSettings();

                if (HSC.Settings.Playlist.Files.IsNullOrEmpty())
                {
                    PluginLog.Information($"No songs in HSC playlist");
                    if (!wasPlaying)
                        PerformActions.DoPerformAction(0);
                    PlaylistManager.CurrentPlaying = -1;
                    return;
                }

                PluginLog.Information($"Updating midibard playlist");
                await PlaylistManager.AddAsync(HSC.Settings.Playlist.Files.ToArray());
                PluginLog.Information($"Added {HSC.Settings.Playlist.Files.Count} files.");

                PluginLog.Information($"switching to {HSC.Settings.Playlist.SelectedIndex} from HSC playlist.");

                if (wasPlaying)
                    PlaylistManager.CurrentPlaying = HSC.Settings.Playlist.SelectedIndex;

                if (!wasPlaying && !loggedIn && !HSC.Settings.Playlist.Loaded)
                    MidiPlayerControl.SwitchSong(HSC.Settings.Playlist.SelectedIndex);

                var curItemSettings = HSC.Settings.PlaylistSettings.Settings[Configuration.config.loadedMidiFile];

                UpdateTracks(Configuration.config.loadedMidiFile, curItemSettings);

                if (!loggedIn)
                    MidiBard.Ui.Open();

                HSC.Settings.Playlist.Loaded = true;
                Thread.Sleep(2000);
                HSC.Settings.Playlist.Loaded = false;

            }

            catch (Exception e)
            {
                PluginLog.Error(e, $"Reloading HSC playlist failed. {e.Message}");
            }
        }
    }
}
