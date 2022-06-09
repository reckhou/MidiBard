using Dalamud.Logging;
using FileWatcherEx;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MidiBard
{
    public partial class MidiBard
    {

        internal static FileWatcherEx.FileWatcherEx hscPlaylistWatcher;

        internal static void CreateHSCPlaylistWatcher()
        {
            string filePath = Path.Join(HSC.Settings.AppSettings.CurrentAppPath, Configuration.config.hscPlayListPath);

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            hscPlaylistWatcher = new FileWatcherEx.FileWatcherEx(filePath);

            hscPlaylistWatcher.NotifyFilter = NotifyFilters.LastWrite;
            hscPlaylistWatcher.Filter = "*.*";
            hscPlaylistWatcher.OnChanged -= FileChanged;
            hscPlaylistWatcher.OnChanged += FileChanged;
            hscPlaylistWatcher.Start();
        }

        internal static async void FileChanged(object sender, FileChangedEvent args)
        {
            try
            {
                if (args.ChangeType == ChangeType.CHANGED)
                {
                    
                    PluginLog.Information($"Playlist file changed '{args.FullPath}'.");

                    HSC.Settings.Playlist.Loaded = false;

                    if (Path.GetExtension(args.FullPath) == ".pl")
                        await HSCPlaylistHelpers.Reload();
                        /*await HSCPlaylistHelpers.ReloadSettings();*/ //needed to update the tracks when selecting a song

                    else if (Path.GetExtension(args.FullPath) == ".json")
                        await HSCPlaylistHelpers.ReloadSettings();
                }
            }
            catch (Exception ex)
            {
                PluginLog.Information($"An error occured trying to load '{args.FullPath}'. Message: {ex.Message}");
            }
        }
    }
}
