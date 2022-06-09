﻿

using MidiBard.Common.IPC;
using MidiBard.Common.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midibard.IPC.Messaging.Handlers.Client.Playlist
{


    public class MessageHandler : Messaging.Handlers.Client.MessageHandler<PlaylistMessage>
    {

        public MessageHandler(NamedPipeClient<MidibardPipeMessage> client) : base(client)
        {

        }

        public event EventHandler ReloadMessageReceived;


        protected override void HandleMessage(PlaylistMessage msg)
        {
            if (msg == null)
                return;

            switch (msg.Type)
            {

                case PlaylistMessageType.Reload:
                    ReloadMessageReceived.Invoke(this, EventArgs.Empty);
                    break;

            }
        }
    }
}
