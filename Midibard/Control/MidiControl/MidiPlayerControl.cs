using System;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Logging;
using Melanchall.DryWetMidi.Interaction;
using MidiBard.Control.CharacterControl;
using static MidiBard.MidiBard;
using System.Collections.Generic;
using FFXIVClientStructs.FFXIV.Client.Game.Group;

namespace MidiBard.Control.MidiControl
{ 

    internal static class MidiPlayerControl
    {

        internal static int playDeltaTime = 0;
    
        public enum e_stat
        {
            Stopped,
            Paused,
            Playing
        }

        public static e_stat _stat = e_stat.Stopped;
        public static int LrcIdx = -1;

        public static List<double> LrcTimeStamps = new List<double>();
        public static void Tick(Dalamud.Game.Framework framework)
        {
            if (_stat != e_stat.Playing)
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
                    var partymember = DalamudApi.api.PartyList.CreatePartyMemberReference(DalamudApi.api.PartyList.GetPartyMemberAddress((int)DalamudApi.api.PartyList.PartyLeaderIndex));
                    bool isLeader = String.Compare(partymember.Name.TextValue, DalamudApi.api.ClientState.LocalPlayer.Name.TextValue) == 0;
                    if (isLeader)
                    {
                        string msg = "";
                        if (idx == 0)
                        {
                            msg = $"/y ♪ Now Playing: {Lrc._lrc.Title} ♪ Artist: {Lrc._lrc.Artist} ♪ Album: {Lrc._lrc.Album} ♪ LyricBy: {Lrc._lrc.LrcBy} ♪";
                        }
                        else
                        {
                            PluginLog.LogVerbose($"{Lrc._lrc.LrcWord[LrcTimeStamps[idx]]}");
                            msg = $"♪ {Lrc._lrc.LrcWord[LrcTimeStamps[idx]]} ♪";
                        }

                        MidiBard.Cbase.Functions.Chat.SendMessage(msg);
                    }
                    LrcIdx = idx;
                }
            }
        }

        static int FindLrcIdx(List<double> TimeStamps)
        {
            if (TimeStamps.Count == 0)
                return -1;

            int idx = -1;
            double timeSpan = CurrentPlayback.GetCurrentTime<MetricTimeSpan>().GetTotalSeconds() - Lrc._lrc.Offset/1000.0f;

            foreach (double TimeStamp in TimeStamps)
            {
                if (timeSpan > TimeStamp)
                {
                    idx++;
                } else
                {
                    break;
                }
            }

            return idx >= TimeStamps.Count ? -1 : idx;
        }

        internal static void Play()
        {
	        playDeltaTime = 0;
            if (CurrentPlayback != null)
            {
                try
                {
                    if (CurrentPlayback.GetCurrentTime<MidiTimeSpan>() == CurrentPlayback.GetDuration<MidiTimeSpan>())
                    {
                        CurrentPlayback.MoveToStart();
                    }

                    DoPlay();
                }
                catch (Exception e)
                {
                    PluginLog.Error(e, "error when try to start playing, maybe the playback has been disposed?");
                }
            }
            else
            {
                if (!PlaylistManager.FilePathList.Any())
                {
                    PluginLog.Information("empty playlist");
                    return;
                }

                if (PlaylistManager.CurrentPlaying < 0)
                {
                    SwitchSong(0, true);
                }
                else
                {
                    SwitchSong(PlaylistManager.CurrentPlaying, true);
                }
            }
        }

        public static void DoPlay()
        {
            if (CurrentPlayback == null)
            {
                return;
            }

            CurrentPlayback.Start();
            try
            {
                LrcTimeStamps = Lrc._lrc.LrcWord.Keys.ToList();
                if (_stat != e_stat.Paused)
                {
                    LrcIdx = -1;
                }
            } catch (Exception e)
            {
                PluginLog.LogError(e.Message);
            }
            _stat = e_stat.Playing;
        }

        internal static void Pause()
        {
            CurrentPlayback?.Stop();
            _stat = e_stat.Paused;
        }


        internal static void PlayPause()
        {
            PluginLog.LogVerbose($"Timespan: {CurrentPlayback.GetCurrentTime<MetricTimeSpan>().GetTotalSeconds()}");
            if (FilePlayback.isWaiting)
            {
                FilePlayback.StopWaiting();
            }
            else
            {
                if (MidiBard.IsPlaying)
                {
                    MidiPlayerControl.Pause();
                }
                else
                {
                    MidiPlayerControl.Play();
                }
            }
        }

        internal static void Stop()
        {
            try
            {
                if (CurrentPlayback == null)
                {
                    CurrentTracks.Clear();
                }
                else
                {
                    CurrentPlayback?.Stop();
                    CurrentPlayback?.MoveToTime(new MidiTimeSpan(0));
                }
            }
            catch (Exception e)
            {
                PluginLog.Warning("Already stopped!");
            }
            finally
            {
                LrcIdx = -1;
                _stat = e_stat.Stopped;
                CurrentPlayback?.Dispose();
                CurrentPlayback = null;
            }
        }

        internal static void Next()
        {
            LrcIdx = -1;
            _stat = e_stat.Stopped;
            if (CurrentPlayback != null)
            {
                try
                {
                    var playing = IsPlaying;
                    CurrentPlayback?.Dispose();
                    CurrentPlayback = null;
                    int next = PlaylistManager.CurrentPlaying;

                    switch ((PlayMode)Configuration.config.PlayMode)
                    {
                        case PlayMode.Single:
                        case PlayMode.SingleRepeat:
                        case PlayMode.ListOrdered:
                            next += 1;
                            break;
                        case PlayMode.ListRepeat:
                            next = (next + 1) % PlaylistManager.FilePathList.Count;
                            break;
                        case PlayMode.Random:
                            if (PlaylistManager.FilePathList.Count > 1)
                            {
                                var r = new Random();
                                do
                                {
                                    next = r.Next(0, PlaylistManager.FilePathList.Count);
                                } while (next == PlaylistManager.CurrentPlaying);
                            }
                            break;
                    }

                    SwitchSong(next, playing);
                }
                catch (Exception e)
                {
                    CurrentPlayback = null;
                    PlaylistManager.CurrentPlaying = -1;
                }
            }
            else
            {
                PlaylistManager.CurrentPlaying += 1;
            }
        }

        internal static void Prev()
        {
            LrcIdx = -1;
            _stat = e_stat.Stopped;
            if (CurrentPlayback != null)
            {
                try
                {
                    var playing = IsPlaying;
                    CurrentPlayback?.Dispose();
                    CurrentPlayback = null;
                    int prev = PlaylistManager.CurrentPlaying;

                    switch ((PlayMode)Configuration.config.PlayMode)
                    {
                        case PlayMode.Single:
                        case PlayMode.SingleRepeat:
                        case PlayMode.ListOrdered:
                            prev -= 1;
                            break;
                        case PlayMode.ListRepeat:
                            if (PlaylistManager.CurrentPlaying == 0)
                            {
                                prev = PlaylistManager.FilePathList.Count - 1;
                            }
                            else
                            {
                                prev -= 1;
                            }
                            break;
                        case PlayMode.Random:
                            if (PlaylistManager.FilePathList.Count > 1)
                            {
                                var r = new Random();
                                do
                                {
                                    prev = r.Next(0, PlaylistManager.FilePathList.Count);
                                } while (prev == PlaylistManager.CurrentPlaying);
                            }
                            break;
                    }

                    SwitchSong(prev, playing);
                }
                catch (Exception e)
                {
                    CurrentPlayback = null;
                    PlaylistManager.CurrentPlaying = -1;
                }
            }
            else
            {
                PlaylistManager.CurrentPlaying -= 1;
            }
        }

        internal static void MoveTime(double timeInSeconds)
        {
            try
            {
                var metricTimeSpan = CurrentPlayback.GetCurrentTime<MetricTimeSpan>();
                var dura = CurrentPlayback.GetDuration<MetricTimeSpan>();
                var totalMicroseconds = metricTimeSpan.TotalMicroseconds + (long)(timeInSeconds * 1_000_000);
                if (totalMicroseconds < 0) totalMicroseconds = 0;
                if (totalMicroseconds > dura.TotalMicroseconds) totalMicroseconds = dura.TotalMicroseconds;
                CurrentPlayback.MoveToTime(new MetricTimeSpan(totalMicroseconds));
            }
            catch (Exception e)
            {
                PluginLog.Warning(e.ToString(), "error when try setting current playback time");
            }
        }

        public static void SwitchSong(int index, bool startPlaying = false)
        {
            LrcIdx = -1;
            _stat = e_stat.Stopped;
            playDeltaTime = 0;
            if (index < 0 || index >= PlaylistManager.FilePathList.Count)
            {
                PluginLog.Error($"SwitchSong: invalid playlist index {index}");
                return;
            }

            PlaylistManager.CurrentPlaying = index;
            Task.Run(async () =>
            {
                await FilePlayback.LoadPlayback(PlaylistManager.CurrentPlaying, startPlaying);
            });
        }

	    internal static void ChangeDeltaTime(int delta)
		{
			if (CurrentPlayback == null || !CurrentPlayback.IsRunning)
			{
				playDeltaTime = 0;
				return;
			}

			var currentTime = CurrentPlayback.GetCurrentTime<MetricTimeSpan>();
			long msTime = currentTime.TotalMicroseconds;
			//PluginLog.LogDebug("curTime:" + msTime);
			if (msTime + delta * 1000 < 0)
			{
				return;
			}
			msTime += delta * 1000;
			MetricTimeSpan newTime = new MetricTimeSpan(msTime);
            //PluginLog.LogDebug("newTime:" + newTime.TotalMicroseconds);
			CurrentPlayback.MoveToTime(newTime);
			playDeltaTime += delta;
		}
	}
}
