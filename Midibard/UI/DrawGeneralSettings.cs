using System;
using System.Numerics;
using ImGuiNET;

namespace MidiBard
{

    public partial class PluginUI
    {
        private readonly string[] _toolTips = {
        "Off: Does not take over game's guitar tone control.",
        "Standard: Standard midi channel and ProgramChange handling, each channel will keep it's program state separately.",
        "Simple: Simple ProgramChange handling, ProgramChange event on any channel will change all channels' program state. (This is BardMusicPlayer's default behavior.)",
        "Override by track: Assign guitar tone manually for each track and ignore ProgramChange events.",
    };

        private void DrawPanelGeneralSettings()
        {
            //ImGui.SliderInt("Playlist size".Localize(), ref Configuration.config.playlistSizeY, 2, 50,
            //	config.playlistSizeY.ToString(), ImGuiSliderFlags.AlwaysClamp);
            //ToolTip("Play list rows number.".Localize());

            //ImGui.SliderInt("Player width".Localize(), ref Configuration.config.playlistSizeX, 356, 1000, config.playlistSizeX.ToString(), ImGuiSliderFlags.AlwaysClamp);
            //ToolTip("Player window max width.".Localize());

            //var inputDevices = InputDevice.GetAll().ToList();
            //var currentDeviceInt = inputDevices.FindIndex(device => device == CurrentInputDevice);

            //if (ImGui.Combo(CurrentInputDevice.ToString(), ref currentDeviceInt, inputDevices.Select(i => $"{i.Id} {i.Name}").ToArray(), inputDevices.Count))
            //{
            //	//CurrentInputDevice.Connect(CurrentOutputDevice);
            //}


            var inputDevices = InputDeviceManager.Devices;

            if (ImGui.BeginCombo("Input Device".Localize(), InputDeviceManager.CurrentInputDevice.DeviceName()))
            {
                if (ImGui.Selectable("None##device", InputDeviceManager.CurrentInputDevice is null))
                {
                    InputDeviceManager.SetDevice(null);
                }

                for (int i = 0; i < inputDevices.Length; i++)
                {
                    var device = inputDevices[i];
                    if (ImGui.Selectable($"{device.Name}##{i}", device.Name == InputDeviceManager.CurrentInputDevice?.Name))
                    {
                        InputDeviceManager.SetDevice(device);
                    }
                }

                ImGui.EndCombo();
            }

            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                InputDeviceManager.SetDevice(null);
            }

            ImGuiUtil.ToolTip("Choose external midi input device. right click to reset.".Localize());

            if (ImGui.Checkbox("Auto restart listening".Localize(), ref Configuration.config.autoRestoreListening))
            {
                Configuration.config.Save();
            }

            ImGuiUtil.ToolTip("Try auto restart listening last used midi device".Localize());
            //ImGui.SameLine(ImGui.GetWindowContentRegionWidth() / 2);
            //ImGui.Checkbox("Auto listening new device".Localize(), ref Configuration.config.autoStartNewListening);
            //ImGuiUtil.ToolTip("Auto start listening new midi input device when idle.".Localize());

            ImGui.SameLine(ImGui.GetWindowContentRegionWidth() / 2);

            ImGui.SetNextItemWidth(ImGui.GetWindowContentRegionWidth() / 3.36f);
            if (ImGuiUtil.EnumCombo("Tone mode".Localize(), ref Configuration.config.GuitarToneMode, _toolTips))
            {
                Configuration.config.Save();
            }

            ImGuiUtil.ToolTip("Choose how MidiBard will handle MIDI channels and ProgramChange events(current only affects guitar tone changing)".Localize());

            if (ImGui.Checkbox("Tracks visualization".Localize(), ref Configuration.config.PlotTracks))
            {
                Configuration.config.Save();
            }

            ImGuiUtil.ToolTip("Draw midi tracks in a new window\nshowing the on/off and actual transposition of each track".Localize());
            ImGui.SameLine(ImGui.GetWindowContentRegionWidth() / 2);

            if (ImGui.Checkbox("Follow playback".Localize() + $" ({timeWindow:F2}s)###followPlayBack", ref Configuration.config.LockPlot))
            {
                Configuration.config.Save();
            }

            if (ImGui.IsItemHovered())
            {
                timeWindow *= Math.Pow(Math.E, ImGui.GetIO().MouseWheel * -0.1);
            }
            ImGuiUtil.ToolTip(
                Configuration.config.LockPlot
                    ? "Lock tracks window and auto following current playback progress\nScroll mouse here to adjust view timeline scale".Localize()
                    : "Lock tracks window and auto following current playback progress".Localize());

            if (ImGui.Checkbox("Auto open MidiBard".Localize(), ref Configuration.config.AutoOpenPlayerWhenPerforming))
            {
                Configuration.config.Save();
            }

            ImGuiUtil.ToolTip("Open MidiBard window automatically when entering performance mode".Localize());
            //ImGui.Checkbox("Auto Confirm Ensemble Ready Check".Localize(), ref Configuration.config.AutoConfirmEnsembleReadyCheck);
            //if (localizer.Language == UILang.CN) HelpMarker("在收到合奏准备确认时自动选择确认。");

            ImGui.SameLine(ImGui.GetWindowContentRegionWidth() / 2);

            if (ImGui.Checkbox("Monitor ensemble".Localize(), ref Configuration.config.MonitorOnEnsemble))
            {
                Configuration.config.Save();
            }

            ImGuiUtil.ToolTip("Auto start ensemble when entering in-game party ensemble mode.".Localize());

            if (ImGui.Checkbox("Auto switch instrument by track name(BMP Rules)".Localize(), ref Configuration.config.bmpTrackNames))
            {
                Configuration.config.Save();
            }
            ImGuiUtil.ToolTip("Transpose/switch instrument based on first enabled midi track name.".Localize());


            if (ImGui.Checkbox("Use HSC override".Localize(), ref Configuration.config.useHscmOverride))
            {
                Configuration.config.Save();
            }
            if (Configuration.config.useHscmOverride && ImGui.InputText("Playlist Path", ref Configuration.config.hscPlayListPath, 1024))
            {
                Configuration.config.Save();
            }
            ImGuiUtil.ToolTip("HSC playlist path".Localize());

            if (ImGui.Checkbox("Auto transpose".Localize(), ref Configuration.config.autoTransposeBySongName))
            {
                Configuration.config.Save();
            }
            ImGuiUtil.ToolTip("Auto transpose notes on demand. If you need this, \nplease add #transpose number# before file name.\nE.g. #-12#demo.mid".Localize());

            ImGui.SameLine(ImGui.GetWindowContentRegionWidth() / 2);
            if (ImGui.Checkbox("Auto post command".Localize(), ref Configuration.config.autoPostPartyChatCommand))
            {
                Configuration.config.Save();
            }
            ImGuiUtil.ToolTip("Post chat command on party channel automatically.".Localize());

            //ImGui.Checkbox("Override guitar tones".Localize(), ref Configuration.config.OverrideGuitarTones);
            //ImGuiUtil.ToolTip("Assign different guitar tones for each midi tracks".Localize());


            ImGuiUtil.ColorPickerWithPalette(1000, "Theme color".Localize(), ref Configuration.config.themeColor, ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar);
            //if (ImGui.ColorEdit4("Theme color".Localize(), ref Configuration.config.themeColor,
            //	ImGuiColorEditFlags.AlphaPreview | ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.NoInputs))
            Configuration.config.themeColorDark = Configuration.config.themeColor * new Vector4(0.25f, 0.25f, 0.25f, 1);
            Configuration.config.themeColorTransparent = Configuration.config.themeColor * new Vector4(1, 1, 1, 0.33f);

            if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
            {
                Configuration.config.themeColor = ImGui.ColorConvertU32ToFloat4(0x9C60FF8E);
                Configuration.config.themeColorDark = Configuration.config.themeColor * new Vector4(0.25f, 0.25f, 0.25f, 1);
                Configuration.config.themeColorTransparent = Configuration.config.themeColor * new Vector4(1, 1, 1, 0.33f);
            }
            ImGui.SameLine();
            ImGui.SetCursorPosX(ImGui.GetCursorPosX() - ImGui.GetStyle().ItemInnerSpacing.X);
            ImGui.TextUnformatted("Theme color".Localize());

            ImGui.SameLine(ImGui.GetWindowContentRegionWidth() / 2);
            ImGui.SetNextItemWidth(ImGui.GetWindowContentRegionWidth() / 3.36f);
            if (ImGui.Combo("UI Language".Localize(), ref Configuration.config.uiLang, uilangStrings, 2))
            {
                MidiBard.Localizer = new Localizer((UILang)Configuration.config.uiLang);
                Configuration.config.Save();
            }

            if (ImGui.Checkbox("Auto switch instrument by file name".Localize(), ref Configuration.config.autoSwitchInstrumentBySongName))
            {
                Configuration.config.Save();
            }

            ImGuiUtil.ToolTip("Auto switch instrument on demand. If you need this, \nplease add #instrument name# before file name.\nE.g. #harp#demo.mid".Localize());
        }
    }
}