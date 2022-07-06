
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
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Interaction;
using Newtonsoft.Json;
using Dalamud.Logging;
using MidiBard.HSC.Models;

namespace MidiBard.HSC
{

    [JsonObject(MemberSerialization.OptIn)]
    public static class Settings
    {
        public const string HscmSettingsFileName = "settings.config";



        public class TrackTransposeInfo
        {
            public int KeyOffset { get; set; }
            public int OctaveOffset { get; set; }
        }

        static Settings()
        {

            PlaylistSettings = new SongSettings();

            Playlist = new MidiBard.HSC.Models.Playlist.Playlist();

            AppSettings = new AppSettings();

            CharConfig = new CharacterConfig();
        }

        public static void Cleanup()
        {
            CharName = null;
            CharIndex = -1;

            HSC.Settings.Playlist?.Clear();
            HSC.Settings.PlaylistSettings?.Clear();

            PercussionNotes?.Clear();
            PercussionTracks?.Clear();
            MappedTracks?.Clear();
            TrackInfo?.Clear();
        }

        [JsonProperty]
        public static AppSettings AppSettings { get; set; }

        public static MidiBard.HSC.Models.Playlist.Playlist Playlist { get; set; }

        public static SongSettings PlaylistSettings { get; set; }

        public static CharacterConfig CharConfig { get; set; }

        public static string CharName { get; set; }

        public static int CharIndex { get; set; }

        public static Dictionary<int, Dictionary<int, bool>> PercussionNotes { get; set; }
        public static Dictionary<int, bool> PercussionTracks { get; set; }

        public static Dictionary<int, TrackTransposeInfo> MappedTracks { get; set; }

       public static Dictionary<int, TrackTransposeInfo> TrackInfo { get; set; }

        public static Dictionary<long, Dictionary<SevenBitNumber, bool>> TrimmedNotes { get; set; }

        public static MidiSequence CurrentSongSettings { get; set; }
        public static int OctaveOffset { get; set; }
        public static int KeyOffset { get; set; }
        public static bool SwitchingInstrument { get; set; }

        public static string CurrentAppPath { get; set; }
        public static bool SwitchInstrumentFailed { get; set; }
        public static ITimeSpan? PrevTime { get; set; }
        public static bool SavedConfig { get; set; }
        public static bool HSCMConfigExists { get; set; }

        public static void Load()
        {

            var filePath = Path.Combine(HSC.Settings.CurrentAppPath, HscmSettingsFileName);
            PluginLog.LogDebug($"Load HSCM Setting: {filePath}");
            var appSettings = FileHelpers.Load<AppSettings>(filePath);

            if (appSettings != null)
            {
                AppSettings = appSettings;
                HSCMConfigExists = true;
            } else
            {
                PluginLog.LogDebug($"HSCM AppSettings not exist: {filePath}");
                HSCMConfigExists = false;
            }
        }

        public static void Save()
        {
            SavedConfig = true;

            var filePath = Path.Combine(HSC.Settings.CurrentAppPath, HscmSettingsFileName);
            PluginLog.LogDebug($"Save HSCM Setting: {filePath}");
            FileHelpers.Save(AppSettings, filePath);

        }

    }
}
