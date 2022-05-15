using System;
using Dalamud.Interface;
using Dalamud.Logging;
using Dalamud.Plugin;
using ImGuiNET;
using MidiBard.Control.MidiControl;
using static MidiBard.ImGuiUtil;

namespace MidiBard
{

    public partial class PluginUI
    {
        private static unsafe void DrawButtonMiniPlayer()
        {
            //mini player

            ImGui.SameLine();
            if (ImGui.Button(((FontAwesomeIcon)(Configuration.config.miniPlayer ? 0xF424 : 0xF422)).ToIconString()))
                Configuration.config.miniPlayer ^= true;

            ToolTip("Mini player".Localize());
        }

        private static unsafe void DrawButtonShowPlayerControl()
        {
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text,
                Configuration.config.showMusicControlPanel
                    ? Configuration.config.themeColor
                    : *ImGui.GetStyleColorVec4(ImGuiCol.Text));

            if (ImGui.Button((FontAwesomeIcon.Music).ToIconString()))
                Configuration.config.showMusicControlPanel ^= true;

            ImGui.PopStyleColor();
            ToolTip("Music control panel".Localize());
        }

        private static unsafe void DrawButtonShowSettingsPanel()
        {
            ImGui.SameLine();
            ImGui.PushStyleColor(ImGuiCol.Text, Configuration.config.showSettingsPanel ? Configuration.config.themeColor : *ImGui.GetStyleColorVec4(ImGuiCol.Text));

            if (ImGui.Button(FontAwesomeIcon.Cog.ToIconString()))
                Configuration.config.showSettingsPanel ^= true;

            ImGui.PopStyleColor();
            ToolTip("Settings panel".Localize());
        }

        private static unsafe void DrawButtonPlayPause()
        {
            var PlayPauseIcon = MidiBard.IsPlaying ? FontAwesomeIcon.Pause.ToIconString() : FontAwesomeIcon.Play.ToIconString();
            if (ImGui.Button(PlayPauseIcon))
            {
                PluginLog.Debug($"PlayPause pressed. wasplaying: {MidiBard.IsPlaying}");
                MidiPlayerControl.PlayPause();
            }
        }

        private static unsafe void DrawButtonStop()
        {
            ImGui.SameLine();
            if (ImGui.Button(FontAwesomeIcon.Stop.ToIconString()))
            {
                if (Configuration.config.autoPostPartyChatCommand)
                {
                    MidiBard.Cbase.Functions.Chat.SendMessage("/p close");
                }

                if (FilePlayback.isWaiting)
                {
                    FilePlayback.CancelWaiting();
                }
                else
                {
                    MidiPlayerControl.Stop();
                }
            }
        }

        private static unsafe void DrawButtonFastForward()
        {
            ImGui.SameLine();
            if (ImGui.Button(((FontAwesomeIcon)0xf050).ToIconString()))
            {
                MidiPlayerControl.Next();
            }

            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                MidiPlayerControl.Prev();
            }
        }

        private static unsafe void DrawButtonPlayMode()
        {
            ImGui.SameLine();
            FontAwesomeIcon icon;
            switch ((PlayMode)Configuration.config.PlayMode)
            {
                case PlayMode.Single:
                    icon = (FontAwesomeIcon)0xf3e5;
                    break;
                case PlayMode.ListOrdered:
                    icon = (FontAwesomeIcon)0xf884;
                    break;
                case PlayMode.ListRepeat:
                    icon = (FontAwesomeIcon)0xf021;
                    break;
                case PlayMode.SingleRepeat:
                    icon = (FontAwesomeIcon)0xf01e;
                    break;
                case PlayMode.Random:
                    icon = (FontAwesomeIcon)0xf074;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (ImGui.Button(icon.ToIconString()))
            {
                Configuration.config.PlayMode += 1;
                Configuration.config.PlayMode %= 5;
            }

            if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            {
                Configuration.config.PlayMode += 4;
                Configuration.config.PlayMode %= 5;
            }

            ToolTip("Playmode: ".Localize() +
                    $"{(PlayMode)Configuration.config.PlayMode}".Localize());
        }
    }
}