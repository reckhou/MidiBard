using Dalamud.Logging;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using MidiBard.Control;
using MidiBard.Control.CharacterControl;
using MidiBard.Control.MidiControl;
using MidiBard.Control.MidiControl.PlaybackInstance;
using MidiBard.HSC;
using MidiBard.Managers.Ipc;
using MidiBard.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MidiBard.MidiBard;

namespace MidiBard.HSCM
{
    internal class HSCMFilePlayback
    {
        private static bool needToCancel { get; set; } = false;

        private static TrackInfo GetTrackInfos(Note[] notes, TrackChunk i, int index)
        {
            var eventsCollection = i.Events;
            var TrackNameEventsText = eventsCollection.OfType<SequenceTrackNameEvent>().Select(j => j.Text.Replace("\0", string.Empty).Trim()).Distinct().ToArray();
            var TrackName = TrackNameEventsText.FirstOrDefault() ?? "Untitled";
            var IsProgramControlled = Regex.IsMatch(TrackName, @"^Program:.+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            var timedNoteOffEvent = notes.LastOrDefault()?.GetTimedNoteOffEvent();
            return new TrackInfo
            {
                //TextEventsText = eventsCollection.OfType<TextEvent>().Select(j => j.Text.Replace("\0", string.Empty).Trim()).Distinct().ToArray(),
                ProgramChangeEventsText = eventsCollection.OfType<ProgramChangeEvent>().Select(j => $"channel {j.Channel}, {j.GetGMProgramName()}").Distinct().ToArray(),
                TrackNameEventsText = TrackNameEventsText,
                HighestNote = notes.MaxElement(j => (int)j.NoteNumber),
                LowestNote = notes.MinElement(j => (int)j.NoteNumber),
                NoteCount = notes.Length,
                DurationMetric = timedNoteOffEvent?.TimeAs<MetricTimeSpan>(CurrentTMap) ?? new MetricTimeSpan(),
                DurationMidi = timedNoteOffEvent?.Time ?? 0,
                TrackName = TrackName,
                IsProgramControlled = IsProgramControlled,
                Index = index
            };
        }

        private static BardPlayback GetFilePlayback(MidiFile midifile, string trackName)
        {
            PluginLog.Information($"[LoadPlayback] -> {trackName} START");
            var stopwatch = Stopwatch.StartNew();

            try
            {
                CurrentTMap = midifile.GetTempoMap();
            }
            catch (Exception e)
            {
                PluginLog.Warning("[LoadPlayback] error when getting file TempoMap, using default TempoMap instead.");
                CurrentTMap = TempoMap.Default;
            }
            PluginLog.Information($"[LoadPlayback] -> {trackName} 1 in {stopwatch.Elapsed.TotalMilliseconds} ms");
            try
            {
                CurrentTracks = midifile.GetTrackChunks()
                    .Where(i => i.Events.Any(j => j is NoteOnEvent))
                    .Select((i, index) =>
                    {
                        var notes = i.GetNotes().ToArray();
                        return (i, GetTrackInfos(notes, i, index));
                    }).ToList();
            }
            catch (Exception exception1)
            {
                PluginLog.Warning(exception1, $"[LoadPlayback] error when parsing tracks, falling back to generated NoteEvent playback.");

                try
                {
                    PluginLog.Debug($"[LoadPlayback] file.Chunks.Count {midifile.Chunks.Count}");
                    var trackChunks = midifile.GetTrackChunks().ToList();
                    PluginLog.Debug($"[LoadPlayback] file.GetTrackChunks.Count {trackChunks.Count}");
                    PluginLog.Debug($"[LoadPlayback] file.GetTrackChunks.First {trackChunks.First()}");
                    PluginLog.Debug($"[LoadPlayback] file.GetTrackChunks.Events.Count {trackChunks.First().Events.Count}");
                    PluginLog.Debug($"[LoadPlayback] file.GetTrackChunks.Events.OfType<NoteEvent>.Count {trackChunks.First().Events.OfType<NoteEvent>().Count()}");

                    CurrentTracks = trackChunks
                        .Where(i => i.Events.Any(j => j is NoteOnEvent))
                        .Select((i, index) =>
                        {
                            var noteEvents = i.Events.Where(i => i is NoteEvent or ProgramChangeEvent or TextEvent);
                            var notes = noteEvents.GetNotes().ToArray();
                            var trackChunk = new TrackChunk(noteEvents);
                            return (trackChunk, GetTrackInfos(notes, trackChunk, index));
                        }).ToList();
                }
                catch (Exception exception2)
                {
                    PluginLog.Error(exception2, "[LoadPlayback] still errors? check your file");
                    throw;
                }
            }
            PluginLog.Information($"[LoadPlayback] -> {trackName} 2 in {stopwatch.Elapsed.TotalMilliseconds} ms");


            var timedEvents = CurrentTracks.Select(i => i.trackChunk).AsParallel()
                .SelectMany((chunk, index) => chunk.GetTimedEvents().Select(e =>
                {
                    var compareValue = e.Event switch
                    {
                    //order chords so they always play from low to high
                        NoteOnEvent noteOn => noteOn.NoteNumber,
                    //order program change events so they always get processed before notes 
                        ProgramChangeEvent => -2,
                    //keep other unimportant events order
                        _ => -1
                    };
                    return (compareValue, timedEvent: new TimedEventWithTrackChunkIndex(e.Event, e.Time, index));
                }))
                .OrderBy(e => e.timedEvent.Time)
                .ThenBy(i => i.compareValue)
                .Select(i => i.timedEvent);
            PluginLog.Information($"[LoadPlayback] -> {trackName} 3 in {stopwatch.Elapsed.TotalMilliseconds} ms");

            Array.Fill(CurrentOutputDevice.Channels, new BardPlayDevice.ChannelState());

            PluginLog.Information($"[LoadPlayback] -> {trackName} 3.1 in {stopwatch.Elapsed.TotalMilliseconds} ms");
            var playback = new BardPlayback(timedEvents, CurrentTMap, new MidiClockSettings { CreateTickGeneratorCallback = () => new HighPrecisionTickGenerator() })
            {
                InterruptNotesOnStop = true,
                Speed = Configuration.config.playSpeed,
                TrackProgram = true,
#if DEBUG
            NoteCallback = (data, time, length, playbackTime) =>
            {
                PluginLog.Verbose($"[NOTE] {new Note(data.NoteNumber)} time:{time} len:{length} time:{playbackTime}");
                return data;
            }
#endif
            };
            PluginLog.Information($"[LoadPlayback] -> {trackName} 4 in {stopwatch.Elapsed.TotalMilliseconds} ms");
            PluginLog.Information($"[LoadPlayback] Channels for {trackName}:");
            for (int i = 0; i < CurrentOutputDevice.Channels.Length; i++)
            {
                uint prog = CurrentOutputDevice.Channels[i].Program;
                PluginLog.Information($"  - [{i}]: {ProgramNames.GetGMProgramName((byte)prog)} ({prog})");
            }

            playback.Finished += Playback_Finished;
            PluginLog.Information($"[LoadPlayback] -> {trackName} OK! in {stopwatch.Elapsed.TotalMilliseconds} ms");

            return playback;
        }


        public static bool LoadPlaybackFromSong(
            string songName,
            bool startPlaying = false,
            bool switchInstrument = true,
            bool hscmProcess = true,
            bool ignoreLoadSettings = false,
            bool revertTime = false
        )
        {
            var entry = MidiBardHSCMPlaylistManager.GetSongByName(songName);

            if (entry == null)
            {
                PluginLog.Error($"Could not playback for '{songName}'");
                return false;
            }

            return LoadPlayback(entry.Value.index, startPlaying, switchInstrument, hscmProcess, ignoreLoadSettings, revertTime);
        }

        private static bool LoadPlayback(
            int index,
            bool startPlaying = false, 
            bool switchInstrument = true, 
            bool hscmProcess = true, 
            bool ignoreLoadSettings = false, 
            bool revertTime = false)
        {
            var wasPlaying = IsPlaying;
            CurrentPlayback?.Dispose();
            CurrentPlayback = null;

            MidiFile midiFile = MidiBardHSCMPlaylistManager.LoadMidiFile(index, hscmProcess);
            if (midiFile == null)
            {
                // delete file if can't be loaded(likely to be deleted locally)
                PluginLog.Debug($"[LoadPlayback] removing {index}");
                //PluginLog.Debug($"[LoadPlayback] removing {PlaylistManager.FilePathList[index].path}");
                PlaylistManager.FilePathList.RemoveAt(index);
                return false;
            }
            else
            {
                CurrentPlayback =  GetFilePlayback(midiFile, PlaylistManager.FilePathList[index].displayName);
                Ui.RefreshPlotData();
                PlaylistManager.CurrentPlaying = index;

                if (!ignoreLoadSettings)
                    HSCMPlaylistManager.ApplySettings(true);//this should allow the HSCM playlist to be looped 

                if (switchInstrument)
                {
                    try
                    {
                        PerformHelpers.SwitchInstrumentFromSong();
                    }
                    catch (Exception e)
                    {
                        PluginLog.Warning(e.ToString());
                    }
                }

                if (DalamudApi.api.PartyList.IsInParty() && Configuration.config.useHscmSendReadyCheck)
                    return true;

                if (MidiBard.CurrentInstrument != 0 && (wasPlaying || startPlaying))
                    Control.MidiControl.MidiPlayerControl.DoPlay();

                if (revertTime && wasPlaying)
                    CurrentPlayback.MoveToTime(HSC.Settings.PrevTime);

                return true;
            }
        }


        private static void Playback_Finished(object sender, EventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    if (MidiBard.AgentMetronome.EnsembleModeRunning)
                    {
                        if (Configuration.config.useHscmCloseOnFinish)
                        {
                            PerformHelpers.WaitUntilChanged(() => !MidiBard.AgentMetronome.EnsembleModeRunning, 100, 5000);
                            HSC.PerformHelpers.ClosePerformance();
                        }
                    }
                    else
                    {
                        if (Configuration.config.useHscmCloseOnFinish)
                            HSC.PerformHelpers.ClosePerformance();
                    }

                    FilePlayback.PerformWaiting(Configuration.config.secondsBetweenTracks);
                    if (needToCancel)
                    {
                        needToCancel = false;
                        return;
                    }

                    switch ((PlayMode)Configuration.config.PlayMode)
                    {
                        case PlayMode.Single:
                            break;

                        case PlayMode.SingleRepeat:
                            CurrentPlayback.MoveToStart();
                            Control.MidiControl.MidiPlayerControl.DoPlay();
                            break;

                        case PlayMode.ListOrdered:
                            if (PlaylistManager.CurrentPlaying + 1 < PlaylistManager.FilePathList.Count)
                            {
                                if (HSCMFilePlayback.LoadPlayback(PlaylistManager.CurrentPlaying + 1, true))
                                {
                                }
                            }

                            break;

                        case PlayMode.ListRepeat:
                            if (PlaylistManager.CurrentPlaying + 1 < PlaylistManager.FilePathList.Count)
                            {
                                if (HSCMFilePlayback.LoadPlayback(PlaylistManager.CurrentPlaying + 1, true))
                                {
                                }
                            }
                            else
                            {
                                if (HSCMFilePlayback.LoadPlayback(0, true))
                                {
                                }
                            }

                            break;

                        case PlayMode.Random:

                            if (PlaylistManager.FilePathList.Count == 1)
                            {
                                CurrentPlayback.MoveToStart();
                                break;
                            }

                            try
                            {
                                var r = new Random();
                                int nexttrack;
                                do
                                {
                                    nexttrack = r.Next(0, PlaylistManager.FilePathList.Count);
                                } while (nexttrack == PlaylistManager.CurrentPlaying);

                                if (HSCMFilePlayback.LoadPlayback(nexttrack, true))
                                {
                                }
                            }
                            catch (Exception exception)
                            {
                                PluginLog.Error(exception, "error when random next");
                            }

                            break;

                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                catch (Exception exception)
                {
                    PluginLog.Error(exception, "Unexpected exception when Playback finished.");
                }
            });
        }
    }
}
