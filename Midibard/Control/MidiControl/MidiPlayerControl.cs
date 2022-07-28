using System;
using System.Linq;
using System.Threading.Tasks;
using Dalamud.Logging;
using Melanchall.DryWetMidi.Interaction;
using MidiBard.Control.CharacterControl;
using MidiBard.Control.MidiControl.PlaybackInstance;
using MidiBard.IPC;
using static MidiBard.MidiBard;
using MidiBard.Managers;
using System.Collections.Generic;
using System.Threading;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using MidiBard.Util.Lyrics;

namespace MidiBard.Control.MidiControl
{

    internal static class MidiPlayerControl
    {
        public static bool SwitchingSong;

        internal static void Play()
        {
            playDeltaTime = 0;

            if (CurrentPlayback == null)
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
            else
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
        }

        public static void DoPlay()
        {
            if (CurrentPlayback == null)
            {
                return;
            }

            CurrentPlayback.Start();
            _stat = e_stat.Playing;

            Lrc.Play();
        }

        internal static void Pause()
        {
            CurrentPlayback?.Stop();
            _stat = e_stat.Paused;
        }

        internal static void PlayPause()
        {
            if (CurrentPlayback == null)
            {
                return;
            }

            if (FilePlayback.isWaiting)
            {
                FilePlayback.StopWaiting();
            }
            else
            {
                if (IsPlaying)
                {
                    Pause();
                    var TimeSpan = CurrentPlayback.GetCurrentTime<MetricTimeSpan>();
                    PluginLog.LogInformation($"Timespan: [{TimeSpan.Minutes}:{TimeSpan.Seconds}:{TimeSpan.Milliseconds}]");
                }
                else
                {
                    Play();
                }
            }
        }

        internal static void Stop()
        {
            try
            {
                if (CurrentPlayback == null)
                {
                    //MidiBard.CurrentPlayback?.TrackInfos.Clear();
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
                Lrc.Stop();
                _stat = e_stat.Stopped;
                CurrentPlayback?.Dispose();
                CurrentPlayback = null;
            }
        }

        internal static void Next()
        {
            Lrc.Stop();
            _stat = e_stat.Stopped;
            if (CurrentPlayback != null)
            {
                try
                {
                    var playing = IsPlaying;
                    CurrentPlayback?.Dispose();
                    CurrentPlayback = null;
                    int next = PlaylistManager.CurrentPlaying;

                    switch ((PlayMode)MidiBard.config.PlayMode)
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
            Lrc.Stop();
            _stat = e_stat.Stopped;
            if (CurrentPlayback != null)
            {
                try
                {
                    var playing = IsPlaying;
                    CurrentPlayback?.Dispose();
                    CurrentPlayback = null;
                    int prev = PlaylistManager.CurrentPlaying;

                    switch ((PlayMode)MidiBard.config.PlayMode)
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

        public static void SwitchSong(int index, bool startPlaying = false, bool switchInstrument = true, bool syncByPartyCommand = false)
        {
            if (SwitchingSong)
            {
                return;
            }

            Lrc.Stop();
            _stat = e_stat.Stopped;
            playDeltaTime = 0;

            if (index < 0 || index >= PlaylistManager.FilePathList.Count)
            {
                PluginLog.Error($"SwitchSong: invalid playlist index {index}");
                return;
            }

            PlaylistManager.CurrentPlaying = index;
            SwitchingSong = true;
            Task.Run(async () =>
            {
                if (!syncByPartyCommand)
                {
                    IPCHandles.LoadPlayback(index);
                }
                await FilePlayback.LoadPlayback(PlaylistManager.CurrentPlaying, startPlaying, switchInstrument);    
                // MUST check the current playback, otherwise IPC thread will stuck waiting for playback
                if (!syncByPartyCommand)
                {
                    if (MidiBard.CurrentPlayback != null)
                    {
                        if (MidiBard.CurrentPlayback?.MidiFileConfig is { } config)
                        {
                            IPCHandles.UpdateMidiFileConfig(config, true);
                        }
                    }
                }

                SwitchingSong = false;
            });
        }

        internal static int playDeltaTime = 0;
        
        public enum e_stat
        {
            Stopped,
            Paused,
            Playing
        }

        public static e_stat _stat = e_stat.Stopped;


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
