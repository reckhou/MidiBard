using Dalamud.Logging;
using MidiBard.HSC.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiBard.Common;
using Melanchall.DryWetMidi.Core;
using static MidiBard.HSC.Settings;
using static MidiBard.HSC.Enums;
using MidiBard.HSC.Music;
using MidiBard.Control.CharacterControl;
using System.Diagnostics;
using playlibnamespace;
using System.Threading;
using Dalamud.Interface.Internal.Notifications;
using MidiBard.DalamudApi;
using MidiBard.Managers.Ipc;
using MidiBard.Control.MidiControl;

namespace MidiBard.HSC
{
    public class PerformHelpers
    {


        public static void ClosePerformance()
        {
            if (HSC.Settings.CharIndex == -1)
            {
                ImGuiUtil.AddNotification(NotificationType.Error, $"Cannot close performance mode. Character config not loaded for '{HSC.Settings.CharName}'.");
                return;
            }

            if (HSC.Settings.SwitchInstrumentFailed)
            {
                ImGuiUtil.AddNotification(NotificationType.Error, "Cannot switch instruments yet. Wait 3 seconds.");
                return;
            }

            ImGuiUtil.AddNotification(NotificationType.Info, "Closing performance mode and stopping playback.");

            if (MidiBard.IsPlaying)
                MidiPlayerControl.Stop();
            //ImGuiUtil.AddNotification(NotificationType.Error, "Cannot close instrument while playing.");

            if (MidiBard.CurrentInstrument == 0)
                return;

            PerformActions.DoPerformAction(0);
            bool success = WaitUntilChanged(() => MidiBard.CurrentInstrument == 0, 100, 3000);

            if (!success)
            {
                SwitchInstrumentFailed();
                PluginLog.Error($"Failed to unequip instrument.");
                return;
            }

            Thread.Sleep(200);
            ImGuiUtil.AddNotification(NotificationType.Success, $"Performance mode closed");
        }

        public static bool SwitchInstrumentFromSong(bool force =false)
        {
            try
            {
                if (HSC.Settings.CharIndex == -1)
                {
                    ImGuiUtil.AddNotification(NotificationType.Error, $"Cannot switch instruments from HSCM playlist for '{HSC.Settings.AppSettings.CurrentSong}'. Character config not loaded for '{HSC.Settings.CharName}'.");
                    return false;
                }

                if (!Configuration.config.switchInstrumentFromHscmPlaylist && !force)
                    return MidiBard.CurrentInstrument != 0;

                if (HSC.Settings.SwitchInstrumentFailed)
                {
                    ImGuiUtil.AddNotification(NotificationType.Error, "Cannot switch instruments yet. Wait 3 seconds.");
                    return false;
                }

                PluginLog.Information($"Instrument switching from HSCM playlist for '{HSC.Settings.AppSettings.CurrentSong}'");
                uint insId = GetInstrumentFromHscPlaylist();
                PluginLog.Information($"Switching to '{((Instrument)insId).ToString()}'");

                bool instrumentEquipped = SwitchTo(insId);

                if (!instrumentEquipped)
                {
                    PluginLog.Error($"Failed to equip instrument for '{HSC.Settings.AppSettings.CurrentSong}'.");
                    return false;
                }

                if (instrumentEquipped && Configuration.config.useHscmSendReadyCheck)
                    SendReadyCheckForPartyLeader();

                return true;
            }
            catch (Exception ex)
            {
                PluginLog.Error($"Error when switching instrument from HSC playlist. Message: {ex.Message}");
                return false;
            }
        }


        public static bool ShouldPlayNote(MidiEvent ev, int trackIndex)
        {
            if (!(ev is NoteEvent))
                return true;

            var noteEv = ev as NoteEvent;

            if (HSC.Settings.PercussionNotes.IsNullOrEmpty())
                return true; 

            //not a percussion note so play anyway
            if (!HSC.Settings.PercussionNotes.ContainsKey(trackIndex))
                return true;

            //percussion note - do percussion logic
            return HSC.Settings.PercussionNotes[trackIndex].ContainsKey((int)noteEv.NoteNumber) && HSC.Settings.PercussionNotes[trackIndex][(int)noteEv.NoteNumber];
        }

        public static int GetGuitarTone(Track track) => (int)PerformanceHelpers.GetInstrumentFromName(track.EnsembleInstrument)-24;

        public static bool HasGuitar(Track track)
        {
            var ins = PerformanceHelpers.GetInstrumentFromName(track.EnsembleInstrument);
            if (ins == null)
                return false;
            return (int)ins.Value >= (int)Instrument.ElectricGuitarOverdriven && (int)ins.Value <= (int)Instrument.ElectricGuitarSpecial;
        }


        private static void SendReadyCheckForPartyLeader()
        {
            if (!api.PartyList.IsInParty() || !api.PartyList.IsPartyLeader() || api.PartyList.Length < 2)
                return;

            //wait for everyone to equip instruments first then send
            PluginLog.Information($"Sending ready check");
            playlib.BeginReadyCheck();

            playlib.ConfirmBeginReadyCheck();
            ImGuiUtil.AddNotification(NotificationType.Success, "$Ready check sent.");
        }

        public static bool WaitUntilChanged(Func<bool> condition, int delay = 100, int timeOut = 3000)
        {
            var sw = Stopwatch.StartNew();

            while (!condition())
            {
                if (condition())
                    return true;

                if ((int)sw.Elapsed.TotalMilliseconds >= timeOut)
                    return false;

                Thread.Sleep(delay);
            }

            return true;
        }

        private static uint GetInstrumentFromHscPlaylist()
        {
            try
            {
                if (HSC.Settings.CharIndex == -1)
                    return 0;

                if (HSC.Settings.CurrentSongSettings == null)
                    return 0;

                if (HSC.Settings.CurrentSongSettings.Tracks.IsNullOrEmpty())
                    return 0;

                var firstTrack = HSC.Settings.CurrentSongSettings.Tracks.Values.FirstOrDefault(t => t.EnsembleMember == HSC.Settings.CharIndex);
                if (firstTrack == null)
                    return 0;

                uint insId = (uint)PerformanceHelpers.GetInstrumentFromName(firstTrack.EnsembleInstrument).Value;

                return insId;
            }

            catch (Exception e)
            {
                PluginLog.Error(e, $"Instrument switching from hsc playlist failed. {e.Message}");
                return 0;
            }
        }

        private static void SwitchInstrumentFailed()
        {
            HSC.Settings.SwitchInstrumentFailed = true;
            Task.Run(() =>
            {
                Thread.Sleep(Configuration.config.switchInstrumentDelay);
                Settings.SwitchInstrumentFailed = false;
            });
        }

        private static bool SwitchTo(uint instrumentId, int timeOut = 3000)
        {
            var sw = Stopwatch.StartNew();

            try
            {
                bool success = false;

                if (MidiBard.CurrentInstrument == 0)
                {
                    PerformActions.DoPerformAction(instrumentId);
                    success = WaitUntilChanged(() => MidiBard.CurrentInstrument == instrumentId, 100, timeOut);

                    if (!success)
                    {
                        SwitchInstrumentFailed();
                        ImGuiUtil.AddNotification(NotificationType.Error, $"Failed to equip instrument '{instrumentId}'.");
                        return false;
                    }

                    Thread.Sleep(200);
                    PluginLog.Information($"instrument switching succeed in {sw.Elapsed.TotalMilliseconds} ms");
                    ImGuiUtil.AddNotification(NotificationType.Success, $"Switched to {MidiBard.InstrumentStrings[instrumentId]}");
                    return true;
                }

                if (MidiBard.CurrentInstrument == instrumentId)
                    return true;

                if (MidiBard.guitarGroup.Contains(MidiBard.CurrentInstrument))
                {
                    if (MidiBard.guitarGroup.Contains((byte)instrumentId))
                    {
                        var tone = (int)instrumentId - MidiBard.guitarGroup[0];
                        playlib.GuitarSwitchTone(tone);

                        return true;
                    }
                }

               PerformActions.DoPerformAction(0);
                success = WaitUntilChanged(() => MidiBard.CurrentInstrument == 0, 100, 3000);

                if (!success)//dont try to equip if failed to unequip previous instrument (should prevent crashes)
                {
                    SwitchInstrumentFailed();
                    ImGuiUtil.AddNotification(NotificationType.Error, $"Failed to unequip current instrument..");
                    return false;
                }

                PerformActions.DoPerformAction(instrumentId);
                success = WaitUntilChanged(() => MidiBard.CurrentInstrument == instrumentId, 100, 3000);

                if (!success)
                {
                    SwitchInstrumentFailed();
                    ImGuiUtil.AddNotification(NotificationType.Error, $"Failed to equip instrument '{instrumentId}'.");
                    return false;
                }

                Thread.Sleep(200);
                PluginLog.Information($"instrument switching succeed in {sw.Elapsed.TotalMilliseconds} ms");
                ImGuiUtil.AddNotification(NotificationType.Success, $"Switched to {MidiBard.InstrumentStrings[instrumentId]}");

                return true;
            }

            catch (Exception e)
            {
                PluginLog.Error(e, $"instrument switching failed in {sw.Elapsed.TotalMilliseconds} ms");
                return false;
            }
            finally
            {

            }
        }
    }
}
