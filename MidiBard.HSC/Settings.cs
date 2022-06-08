
using MidiBard.HSC.Helpers;
using MidiBard.HSC.Models.Playlist;
using MidiBard.HSC.Models.Settings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MidiBard.Common;
using MidiBard.HSC.Music;

namespace MidiBard.HSC
{
    public static class Settings
    {

        private const string AppSettingsFileName = "settings.config";

        public static Dictionary<int, bool> EnabledTracks { get; set; }

        static Settings()
        {
            EnabledTracks = new Dictionary<int, bool>();

            SongSettings = new SongSettings();

            PlaylistSettings = new SongSettings();

            Playlist = new MidiBard.HSC.Models.Playlist.Playlist();

            AppSettings = new AppSettings();

            AppSettings.CurrentAppPath = System.IO.Directory.GetCurrentDirectory();

            Paths.MidiFilePath = Path.Combine(AppSettings.CurrentAppPath, "Midis");

            Paths.PlaylistPath = Path.Combine(AppSettings.CurrentAppPath, "Playlists");

            if (AppSettings.PrevPlaylistPath.IsNullOrEmpty())
                AppSettings.PrevPlaylistPath = MidiBard.HSC.Helpers.AppHelpers.GetAppRelativePath(Paths.PlaylistPath);

            if (AppSettings.PrevMidiPath.IsNullOrEmpty())
                AppSettings.PrevMidiPath = MidiBard.HSC.Helpers.AppHelpers.GetAppRelativePath(Paths.MidiFilePath);



        }

        public static AppSettings AppSettings { get; private set; }

        public static SongSettings SongSettings { get; private set; }

        public static MidiBard.HSC.Models.Playlist.Playlist Playlist { get; set; }

        public static SongSettings PlaylistSettings { get; set; }
        
        public static string CharName { get; set; }

        public static int CharIndex { get; set; }

        public static Dictionary<int, Dictionary<int, bool>> PercussionNotes { get; set; }
        public static Dictionary<int, bool> PercussionTracks { get; set; }

        public static Dictionary<int, Track> MappedTracks { get; set; }

        public static async Task LoadAppSettings()
        {

            var filePath = Path.Combine(HSC.Settings.AppSettings.CurrentAppPath, AppSettingsFileName);

            var appSettings = await Task.Run(() => FileHelpers.Load<AppSettings>(filePath));

            if (appSettings != null)
            {
                if (appSettings.PrevPlaylistPath.IsNullOrEmpty())
                    appSettings.PrevPlaylistPath = MidiBard.HSC.Helpers.AppHelpers.GetAppRelativePath(Paths.PlaylistPath);

                if (appSettings.PrevMidiPath.IsNullOrEmpty())
                    appSettings.PrevMidiPath = MidiBard.HSC.Helpers.AppHelpers.GetAppRelativePath(Paths.MidiFilePath);

                AppSettings = appSettings;

            }
        }

        public static async Task LoadPlaylistSettings()
        {
            var filePath = $"{AppSettings.CurrentAppPath}\\{Playlist.SettingsFile}";
            var songSettings = await Task.Run(() => FileHelpers.Load<SongSettings>(filePath));

            if (songSettings != null)
                PlaylistSettings = songSettings;
        }

        public static void SaveAppSettings()
        {
            try
            {
                var filePath = $"{AppSettings.CurrentAppPath}\\{AppSettingsFileName}";
                 FileHelpers.Save(AppSettings, filePath);
            }
            catch (Exception ex) { }
        }

        public static void SavePlaylistSettings(string filePath = null)
        {
            try
            {
                FileHelpers.Save(PlaylistSettings, filePath ?? $"{AppSettings.CurrentAppPath}\\{Playlist.SettingsFile}");
            }
            catch (Exception ex) { }
        }
    }
}
