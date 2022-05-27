using Dalamud.Logging;
using MidiBard.Common.IPC;
using MidiBard.Common.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard
{
    public partial class MidiBard
    {
        #region IPC for external clients
        const string ClientPipeName = "Midibard.pipe";

        internal static Midibard.IPC.Messaging.Handlers.Client.Playback.MessageHandler playbackMessageHandler;

        internal static Midibard.IPC.Messaging.Handlers.Client.Playlist.MessageHandler playlistMessageHandler;

        internal static NamedPipeClient<MidibardPipeMessage> clientPipe;
        #endregion

        private static void InitIPC()
        {
            PluginLog.Information($"Connecting to IPC server.");

            var pipes = System.IO.Directory.GetFiles(@"\\.\pipe\").Select(p => p.Replace(@"\\.\pipe\", ""));

            if (!pipes.Contains(ClientPipeName))
            {
                PluginLog.Information($"IPC server pipe not found.");
                return;
            }

            PluginLog.Information($"IPC server pipe found.");

            clientPipe = new NamedPipeClient<MidibardPipeMessage>(ClientPipeName);

            clientPipe.Start();

            PluginLog.Information("Connected to IPC server.");

            playbackMessageHandler = new Midibard.IPC.Messaging.Handlers.Client.Playback.MessageHandler(clientPipe);

            playbackMessageHandler.PlayMessageReceived += PlaybackMessageHandler_PlayMessageReceived;


            playlistMessageHandler = new Midibard.IPC.Messaging.Handlers.Client.Playlist.MessageHandler(clientPipe);

            playlistMessageHandler.ReloadMessageReceived += PlaylistMessageHandler_ReloadMessageReceived;

            playlistMessageHandler.ReloadSongMessageReceived += PlaylistMessageHandler_ReloadSongMessageReceived;
        }
    }
}
