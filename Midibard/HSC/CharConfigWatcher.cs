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

        internal static FileWatcherEx.FileWatcherEx charConfigWatcher;

        internal static void CreateCharConfigWatcher()
        {
            string filePath = HSC.Settings.AppSettings.CurrentAppPath;

            charConfigWatcher = new FileWatcherEx.FileWatcherEx(filePath);

            charConfigWatcher.NotifyFilter = NotifyFilters.LastWrite;
            charConfigWatcher.Filter = ".config";
            charConfigWatcher.OnChanged -= CharConfigFileChanged;
            charConfigWatcher.OnChanged += CharConfigFileChanged;
            charConfigWatcher.Start();
        }

        internal static async void CharConfigFileChanged(object sender, FileChangedEvent args)
        {
            try
            {
                if (args.ChangeType == ChangeType.CHANGED)
                {
                    if (Path.GetFileName(args.FullPath).Equals(CharConfigHelpers.CharConfigFileName))
                    {
                        PluginLog.Information($"Character config file changed. updating index '{args.FullPath}'.");

                        HSC.Settings.CharIndex = await CharConfigHelpers.GetCharIndex(HSC.Settings.CharName);

                        PluginLog.Information($"Character index for '{HSC.Settings.CharName}': {HSC.Settings.CharIndex}.");
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.Information($"An error occured trying to load '{args.FullPath}'. Message: {ex.Message}");
            }
        }
    }
}
