using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Logging;
using Dalamud.Plugin;
using Lumina.Data;
using Lumina.Data.Files;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Composing;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.MusicTheory;
using Melanchall.DryWetMidi.Standards;
using MidiBard.Control;
using MidiBard.Control.CharacterControl;
using MidiBard.Control.MidiControl;
using MidiBard.DalamudApi;
using MidiBard.Managers;
using MidiBard.Managers.Agents;
using MidiBard.Managers.Ipc;
using MidiBard.Util;
using playlibnamespace;
using static MidiBard.DalamudApi.api;
using Dalamud.Game.Gui;
using XivCommon;
using MidiBard.HSC;
using System.IO.Pipes;
using System.Threading;
using MidiBard.Common.IPC;
using MidiBard.Common.Messaging.Messages;
using MidiBard.HSC.Helpers;

namespace MidiBard;

public partial class MidiBard : IDalamudPlugin
{
    internal static PluginUI Ui { get; set; }
#if DEBUG
		public static bool Debug = true;
#else
    public static bool Debug = false;
#endif

    private static Mutex configMutex;
    internal static BardPlayDevice CurrentOutputDevice { get; set; }
    internal static MidiFile CurrentOpeningMidiFile { get; }
    internal static Playback CurrentPlayback { get; set; }
    internal static TempoMap CurrentTMap { get; set; }
    internal static List<(TrackChunk trackChunk, TrackInfo trackInfo)> CurrentTracks { get; set; }
    internal static Localizer Localizer { get; set; }
    internal static AgentMetronome AgentMetronome { get; set; }
    internal static AgentPerformance AgentPerformance { get; set; }
    //internal static AgentConfigSystem AgentConfigSystem { get; set; }

    private static bool wasEnsembleModeRunning = false;

    internal static ExcelSheet<Perform> InstrumentSheet;

    public static Instrument[] Instruments;

    internal static string[] InstrumentStrings;

    internal static IDictionary<SevenBitNumber, uint> ProgramInstruments;

    internal static byte CurrentInstrument => Marshal.ReadByte(Offsets.PerformanceStructPtr + 3 + Offsets.InstrumentOffset);
    internal static byte CurrentTone => Marshal.ReadByte(Offsets.PerformanceStructPtr + 3 + Offsets.InstrumentOffset + 1);
    internal static readonly byte[] guitarGroup = { 24, 25, 26, 27, 28 };
    internal static bool PlayingGuitar => guitarGroup.Contains(CurrentInstrument);

    internal static bool IsPlaying => CurrentPlayback?.IsRunning == true;

    public string Name => nameof(MidiBard);
    private static ChatGui _chatGui;

    public static XivCommonBase Cbase;
    public static bool SendReloadPlaylistCMD;

    public unsafe MidiBard(DalamudPluginInterface pi, ChatGui chatGui)
    {
        DalamudApi.api.Initialize(this, pi);

        InstrumentSheet = DataManager.Excel.GetSheet<Perform>();

        Instruments = InstrumentSheet!
            .Where(i => !string.IsNullOrWhiteSpace(i.Instrument) || i.RowId == 0)
            .Select(i => new Instrument(i))
            .ToArray();

        InstrumentStrings = Instruments.Select(i => i.InstrumentString).ToArray();

        PluginLog.Information("<InstrumentStrings>");
        foreach (string s in InstrumentStrings)
        {
            PluginLog.Information(s);
        }
        PluginLog.Information("<InstrumentStrings \\>");

        ProgramInstruments = new Dictionary<SevenBitNumber, uint>();
        foreach (var (programNumber, instrument) in Instruments.Select((i, index) => (i.ProgramNumber, index)))
        {
            ProgramInstruments[programNumber] = (uint)instrument;
        }

        PluginLog.Information("<ProgramInstruments>");
        foreach (var programNumber in ProgramInstruments.Keys)
        {
            PluginLog.Information($"[{programNumber}] {ProgramNames.GetGMProgramName(programNumber)} {ProgramInstruments[programNumber]}");
        }
        PluginLog.Information("<ProgramInstruments \\>");

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        Configuration.Init();
        Configuration.Load();

        Localizer = new Localizer((UILang)Configuration.config.uiLang);

        playlib.init(this);
        OffsetManager.Setup(api.SigScanner);
        GuitarTonePatch.InitAndApply();
        Cbase = new XivCommonBase();


        AgentMetronome = new AgentMetronome(AgentManager.Instance.FindAgentInterfaceByVtable(Offsets.MetronomeAgent));
        AgentPerformance = new AgentPerformance(AgentManager.Instance.FindAgentInterfaceByVtable(Offsets.PerformanceAgent));
        //AgentConfigSystem = new AgentConfigSystem(AgentManager.Instance.FindAgentInterfaceByVtable(Offsets.AgentConfigSystem));
        _ = EnsembleManager.Instance;

#if DEBUG
        _ = NetworkManager.Instance;
        _ = Testhooks.Instance;
#endif
        _chatGui = chatGui;
        _chatGui.ChatMessage += ChatCommand.OnChatMessage;

        Task.Run(() => PlaylistManager.AddAsync(Configuration.config.Playlist.ToArray(), true));

        CurrentOutputDevice =  (Configuration.config.useHscmOverride ? new HSCM.MidiControl.BardPlayDevice() : new BardPlayDevice());
        InputDeviceManager.ScanMidiDeviceThread.Start();

        Ui = new PluginUI();
        PluginInterface.UiBuilder.Draw += Ui.Draw;
        Framework.Update += Tick;
        Framework.Update += MidiPlayerControl.Tick;
        PluginInterface.UiBuilder.OpenConfigUi += () => Ui.Toggle();

        //if (PluginInterface.IsDev) Ui.Open();

        DalamudApi.api.ClientState.Login += ClientState_Login;
        DalamudApi.api.ClientState.Logout += ClientState_Logout;

        configMutex = new Mutex(false, "MidiBard.Mutex");

        if (Configuration.config.useHscmOverride && (DalamudApi.api.ClientState.IsLoggedIn || Configuration.config.hscmOfflineTesting))
            Task.Run(() => InitHSCMOverride());
    }

    private static void ClientState_Logout(object sender, EventArgs e)
    {
        if (Configuration.config.useHscmOverride)
        {
            HSCMCleanup();
        }
    }

    private static void ClientState_Login(object sender, EventArgs e)
    {
        Configuration.LoadPrivate();
        if (Configuration.config.useHscmOverride)
            Task.Run(() => InitHSCMOverride(true));
    }

    private void Tick(Dalamud.Game.Framework framework)
    {
        PerformanceEvents.Instance.InPerformanceMode = AgentPerformance.InPerformanceMode;

        if (SendReloadPlaylistCMD)
        {
            SendReloadPlaylistCMD = false;
            MidiBard.Cbase.Functions.Chat.SendMessage("/p reloadplaylist");
        }

        if (!Configuration.config.MonitorOnEnsemble) return;

        if (AgentPerformance.InPerformanceMode)
        {
            playlib.ConfirmReceiveReadyCheck();

            if (!AgentMetronome.EnsembleModeRunning && wasEnsembleModeRunning)
            {
                //if (Configuration.config.StopPlayingWhenEnsembleEnds)
                //{
                MidiPlayerControl.Stop();
                //}
            }

            wasEnsembleModeRunning = AgentMetronome.EnsembleModeRunning;
        }
    }

    [Command("/midibard")]
    [HelpMessage("Toggle MidiBard window")]
    public void Command1(string command, string args) => OnCommand(command, args);

    [Command("/mbard")]
    [HelpMessage("toggle MidiBard window\n" +
                 "/mbard perform [instrument name|instrument ID] → switch to specified instrument\n" +
                 "/mbard cancel → quit performance mode\n" +
                 "/mbard visual [on|off|toggle] → midi tracks visualization\n" +
                 "/mbard [play|pause|playpause|stop|next|prev|rewind (seconds)|fastforward (seconds)] → playback control" +
                 "Party commands: Type commands below on party chat to control all bards in the party.\n" +
                 "switchto <track number> → Switch to <track number> on the play list. e.g. switchto 3 = Switch to the 3rd song.\n" +
                 "close → Stop playing and exit perform mode.\n" +
                 "reloadplaylist → Reload playlist on all clients from the same PC, use after making any changes on the playlist.")]
    public void Command2(string command, string args) => OnCommand(command, args);

    async Task OnCommand(string command, string args)
    {
        var argStrings = args.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        PluginLog.Debug($"command: {command}, {string.Join('|', argStrings)}");
        if (argStrings.Any())
        {
            switch (argStrings[0])
            {
                case "cancel":
                    PerformActions.DoPerformAction(0);
                    break;
                case "perform":
                    try
                    {
                        var instrumentInput = argStrings[1];
                        if (instrumentInput == "cancel")
                        {
                            PerformActions.DoPerformAction(0);
                        }
                        else if (uint.TryParse(instrumentInput, out var id1) && id1 < InstrumentStrings.Length)
                        {
                            SwitchInstrument.SwitchToContinue(id1);
                        }
                        else if (SwitchInstrument.TryParseInstrumentName(instrumentInput, out var id2))
                        {
                            SwitchInstrument.SwitchToContinue(id2);
                        }
                    }
                    catch (Exception e)
                    {
                        PluginLog.Warning(e, "error when parsing or finding instrument strings");
                        _chatGui.PrintError($"failed parsing command argument \"{args}\"");
                    }

                    break;
                case "playpause":
                    MidiPlayerControl.PlayPause();
                    break;
                case "play":
                    MidiPlayerControl.Play();
                    break;
                case "pause":
                    MidiPlayerControl.Pause();
                    break;
                case "stop":
                    MidiPlayerControl.Stop();
                    break;
                case "next":
                    MidiPlayerControl.Next();
                    break;
                case "prev":
                    MidiPlayerControl.Prev();
                    break;
                case "visual":
                    try
                    {
                        Configuration.config.PlotTracks = argStrings[1] switch
                        {
                            "on" => true,
                            "off" => false,
                            _ => !Configuration.config.PlotTracks
                        };
                    }
                    catch (Exception e)
                    {
                        Configuration.config.PlotTracks ^= true;
                    }
                    break;
                case "rewind":
                    {
                        double timeInSeconds = -5;
                        try
                        {
                            timeInSeconds = -double.Parse(argStrings[1]);
                        }
                        catch (Exception e)
                        {
                        }

                        MidiPlayerControl.MoveTime(timeInSeconds);
                    }
                    break;
                case "fastforward":
                    {
                        double timeInSeconds = 5;
                        try
                        {
                            timeInSeconds = double.Parse(argStrings[1]);
                        }
                        catch (Exception e)
                        {
                        }

                        MidiPlayerControl.MoveTime(timeInSeconds);
                    }
                    break;
            }
        }
        else
        {
            Ui.Toggle();
        }
    }

    public static void DoMutexAction(System.Action action)
    {
        action();
        //bool hasHandle = false;

        //try
        //{
        //    hasHandle = configMutex.WaitOne(5000);
        //    if (hasHandle)
        //    {
        //        PluginLog.Information("GOT THE HANDLE OF TEH MUTEX");
        //        action();
        //        configMutex?.ReleaseMutex();
        //    }
        //}
        //catch (AbandonedMutexException)
        //{
        //    // Log the fact that the mutex was abandoned in another process,
        //    // it will still get acquired
        //    action();
        //    hasHandle = true;
        //}
        //finally
        //{
        //    // edited by acidzombie24, added if statement
        //    if (hasHandle)
        //        configMutex?.ReleaseMutex();
        //}
    }

    #region IDisposable Support

    void FreeUnmanagedResources()
    {
        try
        {
            GuitarTonePatch.Dispose();
            InputDeviceManager.ShouldScanMidiDeviceThread = false;
            Framework.Update -= Tick;
            Framework.Update -= MidiPlayerControl.Tick;
            PluginInterface.UiBuilder.Draw -= Ui.Draw;

            EnsembleManager.Instance.Dispose();
#if DEBUG
            Testhooks.Instance?.Dispose();
			NetworkManager.Instance.Dispose();
#endif
            InputDeviceManager.DisposeCurrentInputDevice();
            try
            {
                CurrentPlayback?.Stop();
                CurrentPlayback?.Dispose();
                CurrentPlayback = null;
            }
            catch (Exception e)
            {
                PluginLog.Error($"{e}");
            }
            DalamudApi.api.Dispose();

            if (Configuration.config.useHscmOverride)
            {
                HSCMCleanup();
            }
        }
        catch (Exception e2)
        {
            PluginLog.Error(e2, "error when disposing midibard");
        }
    }

    public void Dispose()
    {
        _chatGui.ChatMessage -= ChatCommand.OnChatMessage;
        Cbase.Dispose();
        FreeUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    ~MidiBard()
    {
        FreeUnmanagedResources();
    }
    #endregion
}