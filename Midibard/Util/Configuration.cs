using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Dalamud;
using Dalamud.Configuration;
using Dalamud.Logging;
using Dalamud.Plugin;
using ImGuiNET;
using MidiBard.Common;
using Newtonsoft.Json;

namespace MidiBard;

public enum PlayMode
{
    Single,
    SingleRepeat,
    ListOrdered,
    ListRepeat,
    Random
}

public enum GuitarToneMode
{
    Off,
    Standard,
    Simple,
    Override,
}
public enum UILang
{
    EN,
    CN
}

[JsonObject(MemberSerialization.OptOut)]

public class Configuration : IPluginConfiguration
{

    public static Configuration Create(RefConfiguration config)
    {

        Configuration obj = new Configuration();

        var fields = config.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToDictionary(f => f.Name, f => f);
        var props = obj.GetType().GetProperties().ToDictionary(p => p.Name, p => p);

        foreach (var field in fields)
        {
            object val = field.Value.GetValue(config);
            props[field.Key].SetValue(obj, val);
        }

        return obj;
    }

    static Configuration()
    {
        config = new RefConfiguration();
    }

    [Newtonsoft.Json.JsonIgnore]
    public static RefConfiguration config;


    public bool useHscmOverride { get; set; } = true;
    public bool switchInstrumentFromHscmPlaylist { get; set; } = true;
    public bool useHscmChordTrimming { get; set; } = true;
    public bool useHscmTransposing { get; set; } = true;
    public bool useHscmTrimByTrack { get; set; } = false;
    public bool useHscmCloseOnFinish { get; set; } = false;
    public bool useHscmSendReadyCheck { get; set; } = false;
    public bool hscmAutoPlaySong { get; set; } = false;
    public int hscmOverrideDelay { get; set; } = 5000;
    public bool hscmOfflineTesting { get; set; } = false;

    public string hscmMidiFile { get; set; }
    public string hscPlayListPath { get; set; } = "playlists";

    public int prevSelected { get; set; }

    public int Version { get; set; }
    public bool Debug { get; set; }
    public bool DebugAgentInfo { get; set; }
    public bool DebugDeviceInfo { get; set; }
    public bool DebugOffsets { get; set; }
    public bool DebugKeyStroke { get; set; }
    public bool DebugMisc { get; set; }
    public bool DebugEnsemble { get; set; }

    public List<string> Playlist { get; set; } = new List<string>();

    public float playSpeed { get; set; } = 1f;
    public float secondsBetweenTracks { get; set; } = 3;
    public int PlayMode { get; set; } = 0;
    public int TransposeGlobal { get; set; } = 0;
    public bool AdaptNotesOOR { get; set; } = true;

    public bool MonitorOnEnsemble { get; set; } = true;
    public bool AutoOpenPlayerWhenPerforming { get; set; } = true;
    public int? SoloedTrack { get; set; } = null;
    public int[] TonesPerTrack { get; set; } = new int[100];
    public bool EnableTransposePerTrack { get; set; } = false;
    public int[] TransposePerTrack { get; set; } = new int[100];
    public int uiLang { get; set; } = DalamudApi.api.PluginInterface.UiLanguage == "zh" ? 1 : 0;
    public bool showMusicControlPanel { get; set; } = true;
    public bool showSettingsPanel { get; set; } = true;
    public int playlistSizeY { get; set; } = 10;
    public bool miniPlayer { get; set; } = false;
    public bool enableSearching { get; set; } = false;

    public bool autoSwitchInstrumentBySongName { get; set; } = true;
    public bool autoTransposeBySongName { get; set; } = true;

    public bool bmpTrackNames { get; set; } = false;
    public bool autoPostPartyChatCommand { get; set; } = false;

    //public bool autoSwitchInstrumentByTrackName = false;
    //public bool autoTransposeByTrackName = false;


    public Vector4 themeColor { get; set; } = ImGui.ColorConvertU32ToFloat4(0x9C60FF8E);
    public Vector4 themeColorDark { get; set; } = ImGui.ColorConvertU32ToFloat4(0x9C60FF8E) * new Vector4(0.25f, 0.25f, 0.25f, 1);
    public Vector4 themeColorTransparent { get; set; } = ImGui.ColorConvertU32ToFloat4(0x9C60FF8E) * new Vector4(1, 1, 1, 0.33f);

    public bool lazyNoteRelease { get; set; } = true;
    public string lastUsedMidiDeviceName { get; set; } = "";
    public bool autoRestoreListening { get; set; } = false;
    public string lastOpenedFolderPath { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

    //public bool autoStartNewListening = false;

    //public int testLength = 40;
    //public int testInterval;
    //public int testRepeat;

    //public float timeBetweenSongs = 0;

    // Add any other properties or methods here.

    ///////////////////////////////////////////////////////////////////////////////

    public bool useLegacyFileDialog { get; set; }
    public bool PlotTracks { get; set; }
    public bool LockPlot { get; set; }

    //public float plotScale = 10f;


    //public List<EnsembleTrack> EnsembleTracks = new List<EnsembleTrack>();
    public bool StopPlayingWhenEnsembleEnds { get; set; } = false;
    //public bool SyncPlaylist = false;
    //public bool SyncSongSelection = false;
    //public bool SyncMuteUnMute = false;
    public GuitarToneMode GuitarToneMode { get; set; } = GuitarToneMode.Off;
    public int switchInstrumentDelay { get; set; } = 3000;

    [Newtonsoft.Json.JsonIgnore]
    public bool OverrideGuitarTones => GuitarToneMode == GuitarToneMode.Override;

    //public void Save()
    //{
    //    var startNew = Stopwatch.StartNew();
    //    DalamudApi.api.PluginInterface.SavePluginConfig(this);
    //    PluginLog.Verbose($"config saved in {startNew.Elapsed.TotalMilliseconds}.");
    //}

    private static string ConfigFilePath => Path.Combine(DalamudApi.api.PluginInterface.GetPluginConfigDirectory(), $"{nameof(MidiBard)}.json");

    public static void Init()
    {
        config = new RefConfiguration();

        ConfigurationPrivate.Init();
    }

    public static void Save(bool reloadplaylist = false)
    {
        Task.Run(() =>
        {
            try
            {
                var startNew = Stopwatch.StartNew();
                var outConfig = Create(config);

                MidiBard.DoMutexAction(() => {
                    FileHelpers.Save(outConfig, ConfigFilePath);
                    ConfigurationPrivate.config.Save();
                });

                PluginLog.Verbose($"config saved in {startNew.Elapsed.TotalMilliseconds}ms");
                if (reloadplaylist && config.autoPostPartyChatCommand)
                {
                    MidiBard.SendReloadPlaylistCMD = true;
                }
            }
            catch (Exception e)
            {
                PluginLog.Error(e, "Error when saving config");
                ImGuiUtil.AddNotification(Dalamud.Interface.Internal.Notifications.NotificationType.Error, "Error when saving config");
            }
        });
    }

    public static void Load()
    {
        var loadedConfig = FileHelpers.Load<Configuration>(ConfigFilePath) ?? new Configuration();
        config = RefConfiguration.Create(loadedConfig);
        ConfigurationPrivate.Load();
    }

}