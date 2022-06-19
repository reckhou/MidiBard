using Dalamud.Interface.Internal.Notifications;
using Dalamud.Logging;
using MidiBard.HSC;
using SharedMemory;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MidiBard
{
    public partial class MidiBard
    {

        public static Mutex ConfigMutex;

        public static ManualResetEvent WaitEvent;

        public static void SaveConfig()
        {
            //MidiBard.ConfigMutex.WaitOne();
            Configuration.config.Save();
            //MidiBard.ConfigMutex.ReleaseMutex();
        }

        private static void HSCMCleanup()
        {
            try
            {
                StopClientMessageHandler();

                Common.IPC.SharedMemory.Clear();
                Common.IPC.SharedMemory.Close();
                hscmWaitHandle?.Close();

                DalamudApi.api.ClientState.Login -= ClientState_Login;
                DalamudApi.api.ClientState.Logout -= ClientState_Logout;

                hscmFileWatcher?.Stop();
                hscmFileWatcher?.Dispose();
                hscmFileWatcher = null;

                if (msgHandler != null)
                {
                    msgHandler.ChangeSongMessageReceived -= MsgHandler_ChangeSongMessageReceived;
                    msgHandler.ReloadPlaylistMessageReceived -= MsgHandler_ReloadPlaylistMessageReceived;
                    msgHandler.ReloadPlaylistSettingsMessageReceived -= MsgHandler_ReloadPlaylistSettingsMessageReceived;
                    msgHandler.SwitchInstrumentsMessageReceived -= MsgHandler_SwitchInstrumentsMessageReceived;
                    msgHandler.RestartHscmOverrideMessageReceived -= MsgHandler_RestartHscmOverrideMessageReceived;
                    msgHandler.ClosePerformanceMessageReceived -= MsgHandler_ClosePerformanceMessageReceived;

                    msgHandler = null;
                }

                HSC.Settings.Cleanup();
            }
            catch (Exception ex)
            {
                PluginLog.Error($"An error occured on HSC override cleanup. Message: {ex.Message}");

            }
        }

        public static void RestartHSCMOverride()
        {
            HSCMCleanup();
            Thread.Sleep(1000);
            InitHSCMOverride();
        }

        public static void InitHSCMOverride(bool loggedIn = false)
        {

            try
            {
                ImGuiUtil.AddNotification(NotificationType.Info, $"Starting HSCM override.");
                HSC.Settings.CurrentAppPath = DalamudApi.api.PluginInterface.AssemblyLocation.DirectoryName;
                PluginLog.Information($"Current plugin path '{HSC.Settings.CurrentAppPath}'.");

                if (loggedIn)//wait until fully logged in
                    Thread.Sleep(Configuration.config.hscmOverrideDelay);

                ConfigMutex = new Mutex(true, "MidiBard.Mutex");
                WaitEvent = new ManualResetEvent(false);

                CreateHSCMConfigFileWatcher();

                Settings.LoadHSCMSettings();
                PopulateConfigFromMidiBardSettings();
                UpdateClientInfo();

                HSCMPlaylistManager.Reload(loggedIn);
                HSCMPlaylistManager.ReloadSettingsAndSwitch(loggedIn);

                WaitForHscm();

                StartClientMessageHander();

                ImGuiUtil.AddNotification(NotificationType.Success, $"HSCM override started success.");
            }
            catch (Exception ex)
            {
                PluginLog.Error($"An error occured on HSCM override init. Message: {ex.Message}");
            }
        }

        private static void WaitForHscm()
        {
            ImGuiUtil.AddNotification(NotificationType.Info, $"Connecting to HSCM.");
            hscmWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset, $"MidiBard.WaitEvent.{HSC.Settings.CharIndex}");
            if (hscmWaitHandle == null)
            {
                ImGuiUtil.AddNotification(NotificationType.Error, $"Cannot connect to HSCM.");
                PluginLog.Error($"An error occured opening event wait handle.");
                return;
            }
            bool success = hscmWaitHandle.WaitOne();
            ImGuiUtil.AddNotification(NotificationType.Success, $"Connected to HSCM.");
        }


        private static void PopulateConfigFromMidiBardSettings()
        {
            Configuration.config.useHscmChordTrimming = Settings.AppSettings.GeneralSettings.EnableMidiBardTrim;
            Configuration.config.useHscmTrimByTrack = Settings.AppSettings.GeneralSettings.EnableMidiBardTrimFromTracks;
            Configuration.config.useHscmTransposing = Settings.AppSettings.GeneralSettings.EnableMidiBardTranspose;
            Configuration.config.switchInstrumentFromHscmPlaylist = Settings.AppSettings.GeneralSettings.EnableMidiBardInstrumentSwitching;
            Configuration.config.useHscmCloseOnFinish = Settings.AppSettings.GeneralSettings.CloseOnFinish;
            Configuration.config.useHscmSendReadyCheck = Settings.AppSettings.GeneralSettings.SendReadyCheckOnEquip;

            PluginLog.Information($"useHscmChordTrimming: {Configuration.config.useHscmChordTrimming}");
            PluginLog.Information($"useHscmTrimByTrack: {Configuration.config.useHscmTrimByTrack}");
            PluginLog.Information($"useHscmTransposing: {Configuration.config.useHscmTransposing}");
            PluginLog.Information($"switchInstrumentFromHscmPlaylist: {Configuration.config.switchInstrumentFromHscmPlaylist}");
            PluginLog.Information($"useHscmCloseOnFinish: {Configuration.config.useHscmCloseOnFinish}");
            PluginLog.Information($"useHscmSendReadyCheck: {Configuration.config.useHscmSendReadyCheck}");
        }

        private static void UpdateClientInfo()
        {
            if (Configuration.config.hscmOfflineTesting)
            {
                HSC.Settings.CharName = "TEST";
                HSC.Settings.CharIndex = 0;
            }
            else
            {
                HSC.Settings.CharName = DalamudApi.api.ClientState.LocalPlayer?.Name.TextValue;
                HSC.Settings.CharIndex = CharConfigHelpers.GetCharIndex(HSC.Settings.CharName);
            }

            PluginLog.Information($"Client logged in. HSCM client info - index: {HSC.Settings.CharIndex}, character name: '{HSC.Settings.CharName}'.");
        }

    }
}
