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
using System.Threading.Tasks;

namespace MidiBard
{
	public class ChatCommand
	{
		public static void OnChatMessage(XivChatType type, uint senderId, ref SeString sender, ref SeString message, ref bool isHandled)
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

				MidiPlayerControl.SwitchSong(number - 1);
			}
			else if (cmd == "reloadplaylist") // reload the playlist from saved config
			{
				if (MidiBard.IsPlaying)
				{
					PluginLog.LogInformation("Reload playlist is not allowed while playing.");
					return;
				}

				Task.Run(() => PlaylistManager.Reload(MidiBard.config.Playlist.ToArray()));
			}
			else if (cmd == "close") // retract the instrument
			{
				if (MidiBard.IsPlaying)
				{
					PluginLog.LogInformation("Close is not allowed while playing.");
					return;
				}

				SwitchInstrument.SwitchTo(0);
			}
		}
	}
}