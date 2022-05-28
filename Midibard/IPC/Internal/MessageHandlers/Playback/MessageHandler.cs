

using Dalamud.Plugin.Ipc;
using Melanchall.DryWetMidi.Interaction;
using MidiBard.Common.IPC;
using MidiBard.IPC.Internal.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidiBard.IPC.Internal.MessageHandlers.Playback
{


    public class MessageHandler : MessageHandler<PlaybackMessage>
    {

        public MessageHandler(ICallGateSubscriber<MidibardMessage, MidibardMessage> subscriber) : base(subscriber)
        {

        }

        public event EventHandler<double> ChangeTempoMessageReceived;
        public event EventHandler<int> ChangeTimeMessageReceived;

        protected override void HandleMessage(PlaybackMessage msg)
        {
            if (msg == null)
                return;

            switch (msg.Type)
            {

                case PlaybackMessageType.ChangeTempo:
                    ChangeTempoMessageReceived.Invoke(this, (double)msg.Data[0]);
                    break;
                case PlaybackMessageType.ChangeTime:
                    ChangeTimeMessageReceived.Invoke(this, (int)msg.Data[0]);
                    break;
            }
        }
    }
}
