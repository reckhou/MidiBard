using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dalamud;
using Dalamud.Configuration;
using Dalamud.Logging;
using Dalamud.Plugin;
using ImGuiNET;
using MidiBard.Common;
using Newtonsoft.Json;

namespace MidiBard;

public class RefConfiguration
{

    public static RefConfiguration Create(Configuration config)
    {
        RefConfiguration obj = new RefConfiguration();

        var fields = obj.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance).ToDictionary(f => f.Name, f => f);
        var props = config.GetType().GetProperties().ToDictionary(p => p.Name, p => p);

        foreach (var prop in props.ToArray())
        {
            object val = prop.Value.GetValue(config);
            if (fields.ContainsKey(prop.Key))
            {
                fields[prop.Key].SetValue(obj, val);
            } else
            {
                PluginLog.LogInformation($"{prop.Key} doesn't exist in config, skipping...");
            }
        }

        return obj;
    }

    public bool useHscmOverride = false;
    public bool switchInstrumentFromHscmPlaylist = true;
    public bool useHscmChordTrimming = true;
    public bool useHscmTransposing = true;
    public bool useHscmTrimByTrack = false;
    public bool useHscmCloseOnFinish = false;
    public bool useHscmSendReadyCheck = false;
    public bool hscmAutoPlaySong = false;
    public int hscmOverrideDelay = 5000;
    public bool hscmOfflineTesting = false;
    public bool hscmShowUI;
    public bool useHscmSongCache = true;

    public string hscmMidiFile;

    public int prevSelected;

    public int Version;
    public bool Debug;
    public bool DebugAgentInfo;
    public bool DebugDeviceInfo;
    public bool DebugOffsets;
    public bool DebugKeyStroke;
    public bool DebugMisc;
    public bool DebugEnsemble;

    public List<string> Playlist = new List<string>();

    public float playSpeed = 1f;
    public float secondsBetweenTracks = 3;
    public int PlayMode = 0;
    public int TransposeGlobal = 0;
    public bool AdaptNotesOOR = true;

    public bool MonitorOnEnsemble = true;
    public bool AutoOpenPlayerWhenPerforming = true;
    public int? SoloedTrack = null;
    public int[] TonesPerTrack = new int[100];
    public bool EnableTransposePerTrack = false;
    public int[] TransposePerTrack = new int[100];
    public int uiLang = DalamudApi.api.PluginInterface.UiLanguage == "zh" ? 1 : 0;
    public bool showMusicControlPanel = true;
    public bool showSettingsPanel = true;
    public int playlistSizeY = 10;
    public bool miniPlayer = false;
    public bool enableSearching = false;

    public bool autoSwitchInstrumentBySongName = true;
    public bool autoTransposeBySongName = true;

    public bool bmpTrackNames = false;
    public bool autoPostPartyChatCommand = false;

    //public bool autoSwitchInstrumentByTrackName = false;
    //public bool autoTransposeByTrackName = false;


    public Vector4 themeColor = ImGui.ColorConvertU32ToFloat4(0x9C60FF8E);
    public Vector4 themeColorDark = ImGui.ColorConvertU32ToFloat4(0x9C60FF8E) * new Vector4(0.25f, 0.25f, 0.25f, 1);
    public Vector4 themeColorTransparent = ImGui.ColorConvertU32ToFloat4(0x9C60FF8E) * new Vector4(1, 1, 1, 0.33f);

    public bool lazyNoteRelease = true;
    public string lastUsedMidiDeviceName = "";
    public bool autoRestoreListening = false;
    public string lastOpenedFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

    //public bool autoStartNewListening = false;

    //public int testLength = 40;
    //public int testInterval;
    //public int testRepeat;

    //public float timeBetweenSongs = 0;

    // Add any other properties or methods here.

    ///////////////////////////////////////////////////////////////////////////////

    public bool useLegacyFileDialog;
    public bool PlotTracks;
    public bool LockPlot;

    //public float plotScale = 10f;


    //public List<EnsembleTrack> EnsembleTracks = new List<EnsembleTrack>();
    public bool StopPlayingWhenEnsembleEnds = false;
    //public bool SyncPlaylist = false;
    //public bool SyncSongSelection = false;
    //public bool SyncMuteUnMute = false;
    public GuitarToneMode GuitarToneMode = GuitarToneMode.Off;
    public int switchInstrumentDelay = 3000;
  
    public bool OverrideGuitarTones => GuitarToneMode == GuitarToneMode.Override;

}