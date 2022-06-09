using MidiBard.HSC.Helpers;
using MidiBard.HSC.Models.Settings;
using MidiBard.HSC.Music;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.HSC.Playlist
{
    public class Playlist
    {
        private const string PlaylistDefaultFileName = "playlist.pl";

        private static string GetDefaultPlaylistFilePath()
        {
            return Path.Combine($"Playlists", PlaylistDefaultFileName);
        }

        public static void Clear()
        {
            MidiBard.HSC.Settings.Playlist.Clear();
            MidiBard.HSC.Settings.PlaylistSettings.Clear();
        }


        public static void LoadPlaylistSettings(Models.Playlist.Playlist playlist)
        {
            if (string.IsNullOrEmpty(playlist.SettingsFile))
                return;

            string settingsFile = Path.Join(Settings.AppSettings.CurrentAppPath, playlist.SettingsFile);

            var playlistSettings =  FileHelpers.Load<SongSettings>(settingsFile);

            if (playlistSettings != null)
                MidiBard.HSC.Settings.PlaylistSettings = playlistSettings;
        }

        public static async Task LoadSongSettings(string filePath)
        {
            try
            {
                var settings = await Task.Run(() => FileHelpers.Load<MidiSequence>(filePath));

                if (settings == null)
                    return;

                //ignore index
                //var plSettings = Common.Settings.PlaylistSettings.Settings[settings.Info.Title];
                //settings.Index = plSettings.Index;
            }
            catch (Exception ex)
            {
                /*AppendLog("", $"Error: unable to load MIDI settings '{filePath}'.")*/
                
            }
        }

        public static void OpenPlaylist(string playlistFilePath, bool loadSettings = true)
        {
            MidiBard.HSC.Settings.Playlist.Title = Path.GetFileNameWithoutExtension(playlistFilePath);

            if (!File.Exists(playlistFilePath))
                return;

            Settings.AppSettings.PrevPlaylistFileName = playlistFilePath;

            var playlist =  FileHelpers.Load<Models.Playlist.Playlist>(playlistFilePath);

            if (playlist == null || playlist.IsEmpty)
                return;

            MidiBard.HSC.Settings.Playlist = playlist;

            if (loadSettings)
                 LoadPlaylistSettings (playlist);
        }

    }
}
