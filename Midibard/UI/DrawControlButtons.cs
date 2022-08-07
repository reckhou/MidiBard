using System;
using Dalamud.Interface;
using Dalamud.Logging;
using Dalamud.Plugin;
using ImGuiNET;
using MidiBard.Control.MidiControl;
using MidiBard.Managers;
using MidiBard2Preview.Resources;
using MidiBard.Control.CharacterControl;
using static MidiBard.ImGuiUtil;
using MidiBard.IPC;

namespace MidiBard;

public partial class PluginUI
{
	private unsafe void DrawButtonVisualization()
	{
		ImGui.SameLine();
		var color = MidiBard.config.PlotTracks ? MidiBard.config.themeColor : *ImGui.GetStyleColorVec4(ImGuiCol.Text);
		if (IconButton((FontAwesomeIcon)0xf008, "visualizertoggle", Language.icon_button_tooltip_visualization,
				ImGui.ColorConvertFloat4ToU32(color)))
			MidiBard.config.PlotTracks ^= true;
		if (ImGui.IsItemClicked(ImGuiMouseButton.Right))
		{
			_resetPlotWindowPosition = true;
		}
	}

	private unsafe void DrawButtonShowSettingsPanel()
	{
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.Text, MidiBard.Ui.showSettingsPanel ? MidiBard.config.themeColor : *ImGui.GetStyleColorVec4(ImGuiCol.Text));

		if (IconButton(FontAwesomeIcon.Cog, "btnsettingp")) showSettingsPanel ^= true;

		ImGui.PopStyleColor();
		ToolTip(Language.icon_button_tooltip_settings_panel);
	}

	private unsafe void DrawButtonShowEnsembleControl()
	{
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.Text, MidiBard.Ui.ShowEnsembleControlWindow ? MidiBard.config.themeColor : *ImGui.GetStyleColorVec4(ImGuiCol.Text));

		if (IconButton((FontAwesomeIcon)0xF0C0, "btnensemble")) ShowEnsembleControlWindow ^= true;

		ImGui.PopStyleColor();
		ToolTip(Language.icon_button_tooltip_ensemble_panel);
	}

	private unsafe void DrawButtonPlayPause()
	{
		if (MidiBard.AgentMetronome.EnsembleModeRunning)
        {
			return;
        }

		var PlayPauseIcon = MidiBard.IsPlaying ? FontAwesomeIcon.Pause : FontAwesomeIcon.Play;
		if (ImGuiUtil.IconButton(PlayPauseIcon, "playpause"))
		{
			PluginLog.Debug($"PlayPause pressed. wasplaying: {MidiBard.IsPlaying}");
			MidiPlayerControl.PlayPause();
		}
		ImGui.SameLine();
	}

	private unsafe void DrawButtonStop()
	{
		if (IconButton(FontAwesomeIcon.Stop, "btnstop"))
		{
			if (FilePlayback.IsWaiting)
			{
				FilePlayback.CancelWaiting();
			} else if (MidiBard.AgentMetronome.EnsembleModeRunning)
			{
				StopEnsemble();
			} else
            {
				MidiPlayerControl.Stop();
			}	
		}
	}

	private unsafe void DrawButtonFastForward()
	{
		if (MidiBard.AgentMetronome.EnsembleModeRunning)
		{
			return;
		}

		ImGui.SameLine();
		if (IconButton(((FontAwesomeIcon)0xf050), "btnff"))
		{
			MidiPlayerControl.Next();
		}

		if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
		{
			MidiPlayerControl.Prev();
		}
	}

	private unsafe void DrawButtonPlayMode()
	{
		if (MidiBard.AgentMetronome.EnsembleModeRunning)
		{
			return;
		}

		ImGui.SameLine();
		FontAwesomeIcon icon = (PlayMode)MidiBard.config.PlayMode switch
		{
			PlayMode.Single => (FontAwesomeIcon)0xf3e5,
			PlayMode.ListOrdered => (FontAwesomeIcon)0xf884,
			PlayMode.ListRepeat => (FontAwesomeIcon)0xf021,
			PlayMode.SingleRepeat => (FontAwesomeIcon)0xf01e,
			PlayMode.Random => (FontAwesomeIcon)0xf074,
			_ => throw new ArgumentOutOfRangeException()
		};

		if (IconButton(icon, "btnpmode"))
		{
			MidiBard.config.PlayMode += 1;
			MidiBard.config.PlayMode %= 5;
		}

		if (ImGui.IsItemHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right))
		{
			MidiBard.config.PlayMode += 4;
			MidiBard.config.PlayMode %= 5;
		}

		ToolTip(array[MidiBard.config.PlayMode]);
	}

	string[] array = new string[]
	{
		Language.play_mode_single,
		Language.play_mode_single_repeat,
		Language.play_mode_list_ordered,
		Language.play_mode_list_repeat,
		Language.play_mode_random,
	};


	private static void StopEnsemble()
	{
		if (MidiBard.config.playOnMultipleDevices && DalamudApi.api.PartyList.Length > 1)
		{
			MidiBard.Cbase.Functions.Chat.SendMessage("/p close");
		}
		else
		{
			if (MidiBard.AgentMetronome.EnsembleModeRunning)
			{
				IPCHandles.UpdateInstrument(false);
			}
		}
	}
}