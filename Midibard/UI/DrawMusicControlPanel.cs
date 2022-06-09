using System;
using System.Threading.Tasks;
using ImGuiNET;
using Melanchall.DryWetMidi.Interaction;
using MidiBard.Control.CharacterControl;
using MidiBard.Control.MidiControl;
using static MidiBard.ImGuiUtil;

namespace MidiBard
{

    public partial class PluginUI
    {
        private void DrawPanelMusicControl()
        {
            ManualDelay();
            if (MidiPlayerControl.LrcLoaded())
            {
                LRCDeltaTime();
            }

            ComboBoxSwitchInstrument();

            SliderProgress();

            if (ImGui.DragFloat("Speed".Localize(), ref Configuration.config.playSpeed, 0.003f, 0.1f, 10f, GetBpmString(),
                    ImGuiSliderFlags.Logarithmic))
            {
                SetSpeed();
            }

            ToolTip("Set the speed of events playing. 1 means normal speed.\nFor example, to play events twice slower this property should be set to 0.5.\nRight Click to reset back to 1.".Localize());

            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                Configuration.config.playSpeed = 1;
                SetSpeed();
            }


            //ImGui.SetNextItemWidth(ImGui.GetWindowWidth() * 0.5f - ImGui.CalcTextSize("Delay".Localize()).X);
            ImGui.PushItemWidth(ImGui.GetWindowContentRegionWidth() / 3.36f);
            ImGui.DragFloat("Delay".Localize(), ref Configuration.config.secondsBetweenTracks, 0.01f, 0, 60,
                $"{Configuration.config.secondsBetweenTracks:f2} s",
                ImGuiSliderFlags.AlwaysClamp | ImGuiSliderFlags.NoRoundToFormat);
            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                Configuration.config.secondsBetweenTracks = 0;
                Configuration.config.Save();
            }
            ToolTip("Delay time before play next track.".Localize());

            ImGui.SameLine(ImGui.GetWindowContentRegionWidth() / 2);
            if (ImGui.InputInt("Transpose".Localize(), ref Configuration.config.TransposeGlobal, 12))
            {
                Configuration.config.Save();
            }
            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                Configuration.config.TransposeGlobal = 0;
                Configuration.config.Save();
            }
            ToolTip("Transpose, measured by semitone. \nRight click to reset.".Localize());
            ImGui.PopItemWidth();

            if (ImGui.Checkbox("Auto adapt notes".Localize(), ref Configuration.config.AdaptNotesOOR))
            {
                Configuration.config.Save();
            }
            ToolTip("Adapt high/low pitch notes which are out of range\r\ninto 3 octaves we can play".Localize());

            ImGui.SameLine(ImGui.GetWindowContentRegionWidth() / 2);

            if (ImGui.Checkbox("Transpose per track".Localize(), ref Configuration.config.EnableTransposePerTrack))
            {
                Configuration.config.Save();
            }
            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                Array.Clear(Configuration.config.TransposePerTrack, 0, Configuration.config.TransposePerTrack.Length);
                Configuration.config.Save();
            }
            ToolTip("Transpose per track, right click to reset all tracks' transpose offset back to zero.".Localize());
            //ImGui.SameLine();

            //ImGui.SliderFloat("secbetweensongs", ref Configuration.config.timeBetweenSongs, 0, 10,
            //	$"{config.timeBetweenSongs:F2} [{500000 * config.timeBetweenSongs:F0}]", ImGuiSliderFlags.AlwaysClamp);


        }

        private static void SetSpeed()
        {
            Configuration.config.playSpeed = Math.Max(0.1f, Configuration.config.playSpeed);
            var currenttime = MidiBard.CurrentPlayback?.GetCurrentTime(TimeSpanType.Midi);
            if (currenttime is not null)
            {
                MidiBard.CurrentPlayback.Speed = Configuration.config.playSpeed;
                MidiBard.CurrentPlayback?.MoveToTime(currenttime);
            }
        }

        private static string GetBpmString()
        {
            Tempo bpm = null;
            var currentTime = MidiBard.CurrentPlayback?.GetCurrentTime(TimeSpanType.Midi);
            if (currentTime != null)
            {
                bpm = MidiBard.CurrentPlayback?.TempoMap?.GetTempoAtTime(currentTime);
            }

            var label = $"{Configuration.config.playSpeed:F2}";

            if (bpm != null)
                label += $" ({bpm.BeatsPerMinute * Configuration.config.playSpeed:F1} bpm)";
            return label;
        }

        private static void SliderProgress()
        {
            if (MidiBard.CurrentPlayback != null)
            {
                var currentTime = MidiBard.CurrentPlayback.GetCurrentTime<MetricTimeSpan>();
                var duration = MidiBard.CurrentPlayback.GetDuration<MetricTimeSpan>();
                float progress;
                try
                {
                    progress = (float)currentTime.Divide(duration);
                }
                catch (Exception e)
                {
                    progress = 0;
                }

                if (ImGui.SliderFloat("Progress".Localize(), ref progress, 0, 1,
                        $"{(currentTime.Hours != 0 ? currentTime.Hours + ":" : "")}{currentTime.Minutes:00}:{currentTime.Seconds:00}",
                        ImGuiSliderFlags.AlwaysClamp | ImGuiSliderFlags.NoRoundToFormat))
                {
                    MidiBard.CurrentPlayback.MoveToTime(duration.Multiply(progress));
                }

                if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                {
                    MidiBard.CurrentPlayback.MoveToTime(duration.Multiply(0));
                }
            }
            else
            {
                float zeroprogress = 0;
                ImGui.SliderFloat("Progress".Localize(), ref zeroprogress, 0, 1, "0:00", ImGuiSliderFlags.NoInput);
            }

            ToolTip("Set the playing progress. \nRight click to restart current playback.".Localize());
        }

        private static int UIcurrentInstrument;
        private static void ComboBoxSwitchInstrument()
        {
            UIcurrentInstrument = MidiBard.CurrentInstrument;
            if (MidiBard.PlayingGuitar)
            {
                UIcurrentInstrument = MidiBard.AgentPerformance.CurrentGroupTone + MidiBard.guitarGroup[0];
                ;
            }
            if (ImGui.Combo("Instrument".Localize(), ref UIcurrentInstrument, MidiBard.InstrumentStrings,
                    MidiBard.InstrumentStrings.Length, 20))
            {
                SwitchInstrument.SwitchToContinue((uint)UIcurrentInstrument);
            }

            ToolTip("Select current instrument. \nRight click to quit performance mode.".Localize());

            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                SwitchInstrument.SwitchToContinue(0);
                MidiPlayerControl.Pause();
            }
        }

        private static void ManualDelay()
        {
            if (ImGui.Button("-10ms"))
            {
                MidiPlayerControl.ChangeDeltaTime(-10);
            }
            ImGui.SameLine();
            if (ImGui.Button("-2ms"))
            {
                MidiPlayerControl.ChangeDeltaTime(-2);
            }
            ImGui.SameLine();
            if (ImGui.Button("+2ms"))
            {
                MidiPlayerControl.ChangeDeltaTime(2);
            }
            ImGui.SameLine();
            if (ImGui.Button("+10ms"))
            {
                MidiPlayerControl.ChangeDeltaTime(10);
            }
            ImGui.SameLine();
            ImGui.TextUnformatted("Manual Sync: " + $"{MidiPlayerControl.playDeltaTime} ms");
            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                MidiPlayerControl.ChangeDeltaTime(-MidiPlayerControl.playDeltaTime);
            }
            ToolTip("Delay time(ms) add on top of current progress to help sync between bards.");
        }

        private static void LRCDeltaTime()
        {

            if (ImGui.Button("-100ms"))
            {
                MidiPlayerControl.ChangeLRCDeltaTime(-100);
            }
            ImGui.SameLine();           
            if (ImGui.Button("+100ms"))
            {
                MidiPlayerControl.ChangeLRCDeltaTime(100);
            }
            ImGui.SameLine();
            ImGui.TextUnformatted("LRC Sync: " + $"{MidiPlayerControl.LRCDeltaTime} ms");
            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                MidiPlayerControl.ChangeLRCDeltaTime(-MidiPlayerControl.LRCDeltaTime);
            }
            ToolTip("Delay time(ms) add on top of lyrics.");
        }
    }
}