
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

namespace MidiBard.HSC
{
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

        public static AppSettings AppSettings { get; private set; }

        public static MidiBard.HSC.Models.Playlist.Playlist Playlist { get; set; }

        public static SongSettings PlaylistSettings { get; set; }
        
        public static string CharName { get; set; }

        public static int CharIndex { get; set; }

        public static Dictionary<int, Dictionary<int, bool>> PercussionNotes { get; set; }
        public static Dictionary<int, bool> PercussionTracks { get; set; }

        public static Dictionary<int, TrackTransposeInfo> MappedTracks { get; set; }

       public static Dictionary<int, TrackTransposeInfo> TrackInfo { get; set; }

        public static Dictionary<long, Dictionary<SevenBitNumber, bool>> TrimmedNotes { get; set; }

        public static int CurrentSongIndex { get; set; }

        public static string CurrentSong { get; set; }

        public static MidiSequence CurrentSongSettings { get; set; }
        public static int OctaveOffset { get; set; }
        public static int KeyOffset { get; set; }
        public static bool SwitchingInstrument { get; set; }

        public static string CurrentAppPath { get; set; }
        public static bool SwitchInstrumentFailed { get; set; }
    
        public static void LoadHSCMSettings()
        {

            var filePath = Path.Combine(HSC.Settings.CurrentAppPath, HscmSettingsFileName);

            var appSettings = FileHelpers.Load<AppSettings>(filePath);

            if (appSettings != null)
                AppSettings = appSettings;
        }

    }
}
