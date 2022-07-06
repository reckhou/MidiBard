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
using MidiBard.Common;
using MidiBard.IPC.SharedMemory;

namespace MidiBard
{
    public partial class MidiBard
    {
        private static MessageHandler msgHandler;
        private static EventWaitHandle hscmWaitHandle;
        private static EventWaitHandle waitHandle;

        private static bool hscmConnected;
        private static bool hscmOverrideStarted;
        private static bool disconnected;

        private static void StartHscmScanner()
        {
            ImGuiUtil.AddNotification(NotificationType.Info, $"Connecting to HSCM.");

            while (hscmOverrideStarted && DalamudApi.api.ClientState.IsLoggedIn || Configuration.config.hscmOfflineTesting)
            {
                if (!hscmOverrideStarted)
                {
                    PluginLog.Information($"Stopping HSCM scanner.");
                    break;
                }
                  
                bool hscmFound = !ProcessFinder.Find("hscm").IsNullOrEmpty();

                if (!hscmFound)
                {
                    if (!hscmOverrideStarted)
                    {
                        PluginLog.Information($"Stopping HSCM scanner.");
                        break;
                    }

                    if (hscmConnected)
                    {
                        PluginLog.Information("HSCM exited. stopping client message handler.");
                        StopClientMessageHandler();
                    }
                }

                if (hscmFound)
                {
                    if (!hscmOverrideStarted)
                    {
                        PluginLog.Information($"Stopping HSCM scanner.");
                        break;
                    }

                    if (!hscmConnected)
                        TryConnectHscm();
                }
                Thread.Sleep(1000);
            }
        }

        private static void HSCMCleanup()
        {
            try
            {
                PluginLog.Information($"Stopping HSCM override and cleaning up.");

                hscmOverrideStarted = false;

                StopClientMessageHandler();

                Common.IPC.SharedMemory.Close();

                DisposeHSCMConfigFileWatcher();

                HSC.Settings.Cleanup();
            }
            catch (Exception ex)
            {
                PluginLog.Error($"An error occured on HSC override cleanup. Message: {ex.Message}");
            }
        }

        public static void RestartHSCMOverride()
        {
            if (Configuration.config.useHscmOverride)
            {
                HSCMCleanup();
                Thread.Sleep(1000);
                InitHSCMOverride();
            }
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

                CreateHSCMConfigFileWatcher();

                Settings.Load();
                PopulateConfigFromMidiBardSettings();
                UpdateClientInfo();

                HSCM.PlaylistManager.Reload(loggedIn);
                HSCM.PlaylistManager.ReloadSettingsAndSwitch(loggedIn);

                ImGuiUtil.AddNotification(NotificationType.Success, $"HSCM override started success.");

                hscmOverrideStarted = true;

                Task.Run(() => StartHscmScanner());
            }
            catch (Exception ex)
            {
                PluginLog.Error($"An error occured on HSCM override init. Message: {ex.Message}");
            }
        }

        private static void TryConnectHscm()
        {
            try
            {
                hscmWaitHandle = EventWaitHandle.OpenExisting($"HSCM.WaitEvent.{HSC.Settings.CharIndex}");
                waitHandle = EventWaitHandle.OpenExisting($"MidiBard.WaitEvent.{HSC.Settings.CharIndex}");
                if (hscmWaitHandle == null || waitHandle == null)
                    return;
            }
            catch (Exception ex) 
            {
                PluginLog.Error($"An error occured opening wait event. Message: {ex.Message}");
                return;
            }

            try
            {
                bool opened = Common.IPC.SharedMemory.CreateOrOpen();

                if (!opened)
                {
                    //ImGuiUtil.AddNotification(NotificationType.Error, $"Cannot connect to HSCM");
                    PluginLog.Error($"An error occured opening or accessing shared memory.");
                    return;
                }
            }
            catch (Exception ex)
            {
                PluginLog.Error($"An error occured opening or accessing shared memory. Message: {ex.Message}");
                return;
            }

            hscmConnected = true;

            hscmWaitHandle.Set();//signal HSCM we are connected
            ImGuiUtil.AddNotification(NotificationType.Success, $"Connected to HSCM.");

            Task.Run(() => StartClientMessageHander());
        }

        private static void PopulateConfigFromMidiBardSettings()
        {
            if (!Settings.HSCMConfigExists)
            {
                return;
            }

            Configuration.config.useHscmChordTrimming = Settings.AppSettings.GeneralSettings.EnableMidiBardTrim;
            Configuration.config.useHscmTrimByTrack = Settings.AppSettings.GeneralSettings.EnableMidiBardTrimFromTracks;
            Configuration.config.useHscmTransposing = Settings.AppSettings.GeneralSettings.EnableMidiBardTranspose;
            Configuration.config.switchInstrumentFromHscmPlaylist = Settings.AppSettings.GeneralSettings.EnableMidiBardInstrumentSwitching;
            Configuration.config.useHscmCloseOnFinish = Settings.AppSettings.GeneralSettings.CloseOnFinish;
            Configuration.config.useHscmSendReadyCheck = Settings.AppSettings.GeneralSettings.SendReadyCheckOnEquip;
            Configuration.config.hscmAutoPlaySong = Settings.AppSettings.GeneralSettings.AutoPlayOnSelect;
            Configuration.config.useHscmOverride = Settings.AppSettings.GeneralSettings.EnableMidiBardControl;
            Configuration.config.hscmShowUI = Settings.AppSettings.GeneralSettings.ShowMidiBardUI;

            PluginLog.Information($"useHscmChordTrimming: {Configuration.config.useHscmChordTrimming}");
            PluginLog.Information($"useHscmTrimByTrack: {Configuration.config.useHscmTrimByTrack}");
            PluginLog.Information($"useHscmTransposing: {Configuration.config.useHscmTransposing}");
            PluginLog.Information($"switchInstrumentFromHscmPlaylist: {Configuration.config.switchInstrumentFromHscmPlaylist}");
            PluginLog.Information($"useHscmCloseOnFinish: {Configuration.config.useHscmCloseOnFinish}");
            PluginLog.Information($"useHscmSendReadyCheck: {Configuration.config.useHscmSendReadyCheck}");
            PluginLog.Information($"hscmAutoPlaySong: {Configuration.config.hscmAutoPlaySong}");
            PluginLog.Information($"useHscmOverride: {Configuration.config.useHscmOverride}");
            PluginLog.Information($"hscmShowUI: {Configuration.config.hscmShowUI}");

            //try
            //{
            //    waitHandle?.Set();
            //}
            //catch (Exception ex)
            //{
            //    PluginLog.Error($"An error occured populating config from HSCM settings. Message: {ex.Message}");
            //    return;
            //}

            Configuration.Save();
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
                HSC.Settings.CharIndex = CharConfig.GetCharIndex(HSC.Settings.CharName);
            }

            PluginLog.Information($"Client logged in. HSCM client info - index: {HSC.Settings.CharIndex}, character name: '{HSC.Settings.CharName}'.");
        }

    }
}
