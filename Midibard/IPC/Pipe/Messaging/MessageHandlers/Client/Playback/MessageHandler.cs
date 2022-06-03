

using MidiBard.Common.IPC;
using MidiBard.Common.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Midibard.IPC.Messaging.Handlers.Client.Playback
{


    public class MessageHandler : Messaging.Handlers.Client.MessageHandler<PlaybackMessage>
    {

        public MessageHandler(NamedPipeClient<MidibardPipeMessage> client) : base(client)
        {

        }

        public event EventHandler<string> PlayMessageReceived;


        protected override void HandleMessage(PlaybackMessage msg)
        {
            if (msg == null)
                return;

            switch (msg.Type)
            {

                case PlaybackMessageType.Play:
                    PlayMessageReceived.Invoke(this, (string)msg.Data[0]);
                    break;

            }
        }
    }
}
