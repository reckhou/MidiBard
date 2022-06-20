using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Dalamud.Logging;

namespace MidiBard
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
        public static Lrc InitLrc(string LrcPath)
        {
            _lrc = new Lrc();
            Dictionary<double, string> dicword = new Dictionary<double, string>();
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
                PluginLog.LogVerbose($"LRC: {ex.Message}");
            }
            return _lrc;
        }

        /// <summary>
        /// 处理信息(私有方法)
        /// </summary>
        /// <param name="line"></param>
        /// <returns>returns the basic info</returns>
        static string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
        }

        public static bool HasLyric()
        {
            return _lrc == null ? false : (_lrc.LrcWord.Count > 0);
        }
    }
}
