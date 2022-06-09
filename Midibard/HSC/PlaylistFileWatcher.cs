﻿using Dalamud.Logging;
using FileWatcherEx;
using System.IO;

namespace MidiBard
{
    public partial class MidiBard
    {

        internal static FileWatcherEx.FileWatcherEx midiBardPlaylistWatcher;

        internal static void CreateHSCPlaylistWatcher()
        {
            string filePath = Path.Join(HSC.Settings.AppSettings.CurrentAppPath, Configuration.config.hscPlayListPath);

            midiBardPlaylistWatcher = new FileWatcherEx.FileWatcherEx(filePath);

            midiBardPlaylistWatcher.NotifyFilter = NotifyFilters.LastWrite;
            midiBardPlaylistWatcher.Filter = "*.*";
            midiBardPlaylistWatcher.OnChanged -= FileChanged;
            midiBardPlaylistWatcher.OnChanged += FileChanged;
            midiBardPlaylistWatcher.Start();
        }

        internal static async void FileChanged(object sender, FileChangedEvent args)
        {
            if (args.ChangeType == ChangeType.CHANGED)
            {
                PluginLog.Information($"Playlist file changed '{args.FullPath}'.");

                if (Path.GetExtension(args.FullPath) == ".pl")
                    await HSCPlaylistHelpers.Reload();

                else if (Path.GetExtension(args.FullPath) == ".json")
                    await HSCPlaylistHelpers.ReloadSettings();
            }
        }
    }
}
