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
using static MidiBard.MidiBard;

namespace MidiBard
{
	public class ChatCommand
	{
		public static bool IgnoreSwitchSongFlag;
		public static bool IgnoreReloadPlaylist;
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

				// use this to avoid double switching on the client which sends the message by automation.
				if (IgnoreSwitchSongFlag)
                {
					IgnoreSwitchSongFlag = false;
					return;
                }

				int number = -1;
				bool success = Int32.TryParse(strings[1], out number);
				if (!success)
				{
					return;
				}

				MidiPlayerControl.SwitchSong(number - 1);
				Ui.Open();
			}
			else if (cmd == "reloadplaylist") // reload the playlist from saved config
			{
				if (MidiBard.IsPlaying)
				{
					PluginLog.LogInformation("Reload playlist is not allowed while playing.");
					return;
				}

				if (IgnoreReloadPlaylist)
                {
					IgnoreReloadPlaylist = false;
					return;
                }

				Configuration.Load();
				Task.Run(() => PlaylistManager.Add(Configuration.config.Playlist.ToArray(), true));
			}
			else if (cmd == "close") // switch off the instrument
			{
				MidiPlayerControl.Stop();
				SwitchInstrument.SwitchTo(0);
			}
		}
	}
}