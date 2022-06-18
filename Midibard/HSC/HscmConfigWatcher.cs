using Dalamud.Interface.Internal.Notifications;
using Dalamud.Logging;
using FileWatcherEx;
using MidiBard.HSC;
using MidiBard.HSC.Models;
using System;
using System.IO;

namespace MidiBard
{
    public partial class MidiBard
    {

        internal static FileWatcherEx.FileWatcherEx hscmFileWatcher;

        internal static void CreateHSCMConfigFileWatcher()
        {
            string filePath = HSC.Settings.CurrentAppPath;

            hscmFileWatcher = new FileWatcherEx.FileWatcherEx(filePath);

            hscmFileWatcher.NotifyFilter = NotifyFilters.LastWrite;

            hscmFileWatcher.OnChanged += HSCMConfigFileChanged;
            hscmFileWatcher.OnCreated += HSCMConfigFileChanged;
            hscmFileWatcher.Start();
        }
        private static void HandleFileChangedOrCreated(string path)
        {
            PluginLog.Information($"File '{path}' changed.");

            if (Path.GetFileName(path).Equals(CharConfigHelpers.CharConfigFileName))
            {
                ImGuiUtil.AddNotification(NotificationType.Info, $"Reloading HSCM character config file.");
                HSC.Settings.CharIndex = CharConfigHelpers.GetCharIndex(HSC.Settings.CharName);
                PluginLog.Information($"Character index for '{HSC.Settings.CharName}': {HSC.Settings.CharIndex}.");
            }
            else if (Path.GetFileName(path).Equals(Settings.HscmSettingsFileName))
            {
                ImGuiUtil.AddNotification(NotificationType.Info, $"Reloading HSCM settings file.");
                Settings.LoadHSCMSettings();
                PopulateConfigFromMidiBardSettings();
                PluginLog.Information($"HSCM settings file loaded.");
            }
        }
        internal static void HSCMConfigFileCreated(object sender, FileChangedEvent args)
        {
            try
            {
                if (args.ChangeType == ChangeType.CREATED)
                    HandleFileChangedOrCreated(args.FullPath);
            }
            catch (Exception ex)
            {
                PluginLog.Error($"An error occured. Message: {ex.Message}");
            }
        }

        internal static void HSCMConfigFileChanged(object sender, FileChangedEvent args)
        {
            try
            {
                if (args.ChangeType == ChangeType.CHANGED)
                    HandleFileChangedOrCreated(args.FullPath);
            }
            catch (Exception ex)
            {
                PluginLog.Error($"An error occured. Message: {ex.Message}");
            }
        }
    }
}
