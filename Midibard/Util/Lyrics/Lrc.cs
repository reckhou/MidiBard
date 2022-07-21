using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using Dalamud.Logging;
using MidiBard.Control.MidiControl;
using MidiBard.Managers.Agents;
using MidiBard.Managers.Ipc;
using MidiBard.DalamudApi;
using Melanchall.DryWetMidi.Interaction;

namespace MidiBard.Util.Lyrics
{
    public class Lrc
    {
        public static Lrc _lrc;
        /// <summary>
        /// 歌曲
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 艺术家
        /// </summary>
        public string Artist { get; set; }
        /// <summary>
        /// 专辑
        /// </summary>
        public string Album { get; set; }
        /// <summary>
        /// 歌词作者
        /// </summary>
        public string LrcBy { get; set; }
        /// <summary>
        /// 偏移量
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// 歌词
        /// </summary>
        public Dictionary<double, string> LrcWord = new Dictionary<double, string>();

        /// <summary>
        /// Get lyric info
        /// </summary>
        /// <param name="LrcPath">path of lrc file</param>
        /// <returns>returns lyrics info</returns>
        public static void InitLrc(string midiFilePath)
        {
            _lrc = new Lrc();
            bool loadSuccessfull = true;
            Dictionary<double, string> dicword = new Dictionary<double, string>();

            string[] pathArray = midiFilePath.Split("\\");
            string LrcPath = "";
            string fileName = Path.GetFileNameWithoutExtension(midiFilePath) + ".lrc";
            for (int i = 0; i < pathArray.Length - 1; i++)
            {
                LrcPath += pathArray[i];
                LrcPath += "\\";
            }

            LrcPath += fileName;

            try
            {
                using (FileStream fs = new FileStream(LrcPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    string line;
                    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            if (line.StartsWith("[ti:"))
                            {
                                _lrc.Title = SplitInfo(line);
                            }
                            else if (line.StartsWith("[ar:"))
                            {
                                _lrc.Artist = SplitInfo(line);
                            }
                            else if (line.StartsWith("[al:"))
                            {
                                _lrc.Album = SplitInfo(line);
                            }
                            else if (line.StartsWith("[by:"))
                            {
                                _lrc.LrcBy = SplitInfo(line);
                            }
                            else if (line.StartsWith("[offset:"))
                            {
                                long offset;
                                long.TryParse(SplitInfo(line), out offset);
                                _lrc.Offset = offset;
                            }
                            else
                            {
                                try
                                {
                                    Regex regexword = new Regex(@".*\](.*)");
                                    Match mcw = regexword.Match(line);
                                    string word = mcw.Groups[1].Value;
                                    Regex regextime = new Regex(@"\[([0-9.:]*)\]", RegexOptions.Compiled);
                                    MatchCollection mct = regextime.Matches(line);
                                    foreach (Match item in mct)
                                    {
                                        double time = TimeSpan.Parse("00:" + item.Groups[1].Value).TotalSeconds;
                                        dicword.Add(time, word);
                                    }
                                }
                                catch
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                _lrc.LrcWord = dicword.OrderBy(t => t.Key).ToDictionary(t => t.Key, p => p.Value);
            }
            catch (Exception ex)
            {
                loadSuccessfull = false;
                PluginLog.LogError(ex.ToString());
            }

            if (loadSuccessfull)
            {
                PluginLog.Log($"Load LRC: {LrcPath}");
                LrcTimeStamps = _lrc.LrcWord.Keys.ToList();
                api.ChatGui.Print(string.Format("[MidiBard 2] Lyrics Loaded: {0}", LrcPath));
            }
        }

        /// <param name="line"></param>
        /// <returns>returns the basic info</returns>
        static string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']').TrimStart();
        }

        public static bool HasLyric()
        {
            return _lrc == null ? false : _lrc.LrcWord.Count > 0;
        }

        public static int LrcIdx = -1;
        internal static int LRCDeltaTime = 50;
        static Stopwatch LRCStopWatch;
        static bool EnsembleInFirst2Measures;

        public static List<double> LrcTimeStamps = new List<double>();

        public static bool LrcLoaded()
        {
            return api.PartyList.IsPartyLeader() && LrcTimeStamps.Count > 0;
        }

        public static void Play()
        {
            LRCDeltaTime = 100; // Assume usual delay between sending and other clients receiving the message would be ~100ms

            if (HasLyric())
            {
                if (DalamudApi.api.PartyList.Length <= 1)
                {
                    DalamudApi.api.ChatGui.Print(string.Format("[MidiBard 2] Not in a party, Lyrics will not be posted."));
                }
            }


            try
            {
                LrcTimeStamps = _lrc.LrcWord.Keys.ToList();
                if (MidiPlayerControl._stat != MidiPlayerControl.e_stat.Paused)
                {
                    LrcIdx = -1;
                }
            }
            catch (Exception e)
            {
                PluginLog.LogError(e.Message);
            }
        }

        public static void Stop()
        {
            LrcIdx = -1;
            EnsembleInFirst2Measures = false;
        }

        internal static void ChangeLRCDeltaTime(int delta)
        {
            if (MidiBard.CurrentPlayback == null || !MidiBard.CurrentPlayback.IsRunning)
            {
                LRCDeltaTime = 100;
                return;
            }

            LRCDeltaTime += delta;
        }

        public static void EnsembleStart()
        {
            EnsembleInFirst2Measures = true;
            LRCStopWatch = Stopwatch.StartNew();
        }

        public static void Ensemble2MeasuresElapsed(int compensation)
        {
            EnsembleInFirst2Measures = false;
            LRCStopWatch.Stop();
            _lrc.Offset += LRCStopWatch.ElapsedMilliseconds - compensation;
        }

        public static void Tick(Dalamud.Game.Framework framework)
        {
            try
            {
                if (MidiPlayerControl._stat != MidiPlayerControl.e_stat.Playing || EnsembleInFirst2Measures)
                {
                    return;
                }

                if (LrcTimeStamps.Count > 0 && LrcIdx < LrcTimeStamps.Count)
                {
                    int idx = FindLrcIdx(LrcTimeStamps);
                    if (idx < 0 || idx == LrcIdx)
                    {
                        return;
                    }
                    else
                    {
                        if (api.PartyList.IsPartyLeader())
                        {
                            string msg = "";
                            if (idx == 0)
                            {
                                msg = $"♪ {_lrc.Title} ♪ ";
                                msg += _lrc.Artist != null && _lrc.Artist != "" ? $"Artist: {_lrc.Artist} ♪ " : "";
                                msg += _lrc.Album != null && _lrc.Album != "" ? $"Album: {_lrc.Album} ♪ " : "";
                                msg += _lrc.LrcBy != null && _lrc.LrcBy != "" ? $"Lyric By: {_lrc.LrcBy} ♪ " : "";

                                if (!MidiBard.AgentMetronome.EnsembleModeRunning)
                                {
                                    msg = "/p " + msg;
                                }
                            }
                            else
                            {
                                PluginLog.LogVerbose($"{_lrc.LrcWord[LrcTimeStamps[idx]]}");
                                if (MidiBard.AgentMetronome.EnsembleModeRunning)
                                {
                                    msg = $"/s ♪ {_lrc.LrcWord[LrcTimeStamps[idx]]} ♪";
                                }
                                else
                                {
                                    msg = $"/p ♪ {_lrc.LrcWord[LrcTimeStamps[idx]]} ♪";
                                }
                            }

                            MidiBard.Cbase.Functions.Chat.SendMessage(msg);
                        }
                        LrcIdx = idx;
                    }
                }
            }
            catch (Exception ex)
            {
                PluginLog.LogError($"exception: {ex}");
            }
        }

        static int FindLrcIdx(List<double> TimeStamps)
        {
            if (TimeStamps.Count == 0)
                return -1;

            int idx = -1;
            double timeSpan = MidiBard.CurrentPlayback.GetCurrentTime<MetricTimeSpan>().TotalSeconds - _lrc.Offset / 1000.0f + LRCDeltaTime / 1000.0f;
            if (timeSpan < 0)
            {
                return -1;
            }

            foreach (double TimeStamp in TimeStamps)
            {
                if (timeSpan > TimeStamp)
                {
                    idx++;
                }
                else
                {
                    break;
                }
            }

            return idx >= TimeStamps.Count ? -1 : idx;
        }
    }
}
