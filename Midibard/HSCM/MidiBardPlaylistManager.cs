using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dalamud.Logging;
using Dalamud.Plugin;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Tools;
using MidiBard.Control.CharacterControl;
using MidiBard.DalamudApi;
using MidiBard.HSC;
using MidiBard.Managers.Ipc;
using Newtonsoft.Json;

namespace MidiBard.HSCM
{

    static class MidiBardPlaylistManager
    {

        public struct SongEntry
        {
            public int index { get; set; }
            public string name { get; set; }
        }

        public static void Add(string[] filePaths)
        {

            var count = filePaths.Length;
            var success = 0;

            filePaths = filePaths.ToArray().Where(p => !Managers.PlaylistManager.FilePathList.Select(f => f.path).Contains(p)).ToArray();

            foreach (var path in filePaths)
            {
                try
                {
                    string fileName = Path.GetFileNameWithoutExtension(path);
                    Configuration.config.Playlist.Add(path);
                    Managers.PlaylistManager.FilePathList.Add((path, fileName, fileName));

                    success++;
                }
                catch { }
            }
            try
            {
                MidiBard.DoMutexAction(() => Configuration.Save());
            }
            catch { }
            PluginLog.Information($"File import all complete! success: {success} total: {count}");
        }

        public static SongEntry? GetSongByName(string name)
        {
            var song = Managers.PlaylistManager.FilePathList.ToArray()
                .Select((fp, i) => new SongEntry { index = i, name = fp.fileName })
                .FirstOrDefault(fp => fp.name.ToLower().Equals(name.ToLower()));

            if (song.Equals(default(SongEntry)))
                return null;

            return song;
        }
    }
}