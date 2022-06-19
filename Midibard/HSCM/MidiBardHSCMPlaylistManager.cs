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

    static class MidiBardHSCMPlaylistManager
    {
        internal static readonly ReadingSettings readingSettings = new ReadingSettings
        {
            NoHeaderChunkPolicy = NoHeaderChunkPolicy.Ignore,
            NotEnoughBytesPolicy = NotEnoughBytesPolicy.Ignore,
            InvalidChannelEventParameterValuePolicy = InvalidChannelEventParameterValuePolicy.ReadValid,
            InvalidChunkSizePolicy = InvalidChunkSizePolicy.Ignore,
            InvalidMetaEventParameterValuePolicy = InvalidMetaEventParameterValuePolicy.SnapToLimits,
            MissedEndOfTrackPolicy = MissedEndOfTrackPolicy.Ignore,
            UnexpectedTrackChunksCountPolicy = UnexpectedTrackChunksCountPolicy.Ignore,
            ExtraTrackChunkPolicy = ExtraTrackChunkPolicy.Read,
            UnknownChunkIdPolicy = UnknownChunkIdPolicy.ReadAsUnknownChunk,
            SilentNoteOnPolicy = SilentNoteOnPolicy.NoteOff,
            TextEncoding = Configuration.config.uiLang == 1 ? Encoding.GetEncoding("gb18030") : Encoding.Default,
            InvalidSystemCommonEventParameterValuePolicy = InvalidSystemCommonEventParameterValuePolicy.SnapToLimits
        };

        public struct SongEntry
        {
            public int index { get; set; }
            public string name { get; set; }
        }

        public static SongEntry? GetSongByName(string name)
        {
            var song = PlaylistManager.FilePathList
                .Select((fp, i) => new SongEntry { index = i, name = fp.fileName })
                .FirstOrDefault(fp => fp.name.ToLower().Equals(name.ToLower()));

            if (song.Equals(default(SongEntry)))
                return null;

            return song;
        }

        internal static MidiFile LoadMidiFile(int index, bool process = false)
        {
            if (index < 0 || index >= PlaylistManager.FilePathList.Count)
            {
                return null;
            }

            //return await LoadMMSongFile(FilePathList[index].path);

            if (Path.GetExtension(PlaylistManager.FilePathList[index].path).Equals(".mid"))
                return LoadMidiFile(PlaylistManager.FilePathList[index].path, process);
            else
                return null;
        }

        internal static MidiFile LoadMidiFile(string filePath, bool process = false)
        {
            PluginLog.Debug($"[LoadMidiFile] -> {filePath} START");
            MidiFile loaded = null;
            var stopwatch = Stopwatch.StartNew();
   
                try
                {
                    if (!File.Exists(filePath))
                    {
                        PluginLog.Warning($"File not exist! path: {filePath}");
                        return null;
                    }
                    string fileName = Path.GetFileNameWithoutExtension(filePath);

                    using (var f = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        loaded = MidiFile.Read(f, readingSettings);
                        if (process && Configuration.config.useHscmOverride)
                            MidiProcessor.Process(loaded, fileName);
                    }

                    PluginLog.Debug($"[LoadMidiFile] -> {filePath} OK! in {stopwatch.Elapsed.TotalMilliseconds} ms");
                }
                catch (Exception ex)
                {
                    PluginLog.Warning(ex, "Failed to load file at {0}", filePath);
                }


            return loaded;
        }

    }
}