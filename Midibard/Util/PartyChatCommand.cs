using System;
using System.Diagnostics;
using Dalamud.Game.Text;
using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Dalamud.Plugin;
using Dalamud.Logging;
using Melanchall.DryWetMidi.Interaction;
using MidiBard.Control.MidiControl;
using MidiBard.Control.CharacterControl;
using MidiBard.IPC;
using MidiBard.Managers.Ipc;
using System.Threading.Tasks;
using static MidiBard.MidiBard;

namespace MidiBard
{
	internal class PartyChatCommand
	{
		internal static void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
		{
			if (isHandled)
				return;

			if (type != XivChatType.Party)
			{
				return;
			}

			string[] strings = message.ToString().Split(' ');
			if (strings.Length < 1)
			{
				return;
			}

			string cmd = strings[0].ToLower();
			if (cmd == "switchto") // switchto + <song number in playlist>
			{
				if (strings.Length < 2)
				{
					return;
				}

				int number = -1;
				bool success = Int32.TryParse(strings[1], out number);
				if (!success)
				{
					return;
				}

				MidiPlayerControl.SwitchSong();
				PlaylistManager.LoadPlayback(number-1);
				Ui.Open();
			}
            else if (cmd == "reloadconfig") // reload the config
            {
				IPCHandles.SyncAllSettings();
			} else if (cmd == "reloadplaylist")
            {
				// hacky way to reload the opening play list
				PlaylistManager.CurrentContainer = PlaylistManager.LoadLastPlaylist();
            }
            else if (cmd == "close") // switch off the instrument
			{
				MidiPlayerControl.Stop();
				SwitchInstrument.SwitchToAsync(0);
			} else if (cmd == "speed")
            {
				if (strings.Length < 2)
				{
					return;
				}

				float number = -1;
				bool success = float.TryParse(strings[1], out number);
				if (!success)
				{
					return;
				}

				MidiBard.config.PlaySpeed = Math.Max(0.1f, number);
			} else if (cmd == "transpose")
            {
				if (strings.Length < 2)
				{
					return;
				}

				int number = -1;
				bool success = Int32.TryParse(strings[1], out number);
				if (!success)
				{
					return;
				}

				MidiBard.config.SetTransposeGlobal(number);
			} else if (cmd == "playonmultipledevices" || cmd == "pmd")
            {
				if(strings.Length < 2)
				{
					return;
				}

				if (strings[1].ToLower() == "on")
                {
					MidiBard.config.playOnMultipleDevices = true;
                } else if (strings[1].ToLower() == "off")
                {
					MidiBard.config.playOnMultipleDevices = false;
                }
			}
		}

		internal static void SendClose()
		{
			if (!MidiBard.config.playOnMultipleDevices || DalamudApi.api.PartyList.Length < 2)
			{
				return;
			}

			MidiBard.Cbase.Functions.Chat.SendMessage("/p close");
		}

		internal static void SendSwitchTo(int songNumber)
        {
			if (!MidiBard.config.playOnMultipleDevices || DalamudApi.api.PartyList.Length < 2)
			{
				return;
			}

			MidiBard.Cbase.Functions.Chat.SendMessage($"/p switchto {songNumber}");
		}

		internal static void SendPMD(bool isOn)
		{
			if (DalamudApi.api.PartyList.Length < 2)
			{
				return;
			}

			var str = isOn ? "on" : "off";
			MidiBard.Cbase.Functions.Chat.SendMessage($"/p pmd {str}");
		}

		internal static void SendReloadPlaylist()
        {
			if (DalamudApi.api.PartyList.Length < 2)
			{
				return;
			}

			MidiBard.Cbase.Functions.Chat.SendMessage($"/p reloadplaylist");
		}
	}
}