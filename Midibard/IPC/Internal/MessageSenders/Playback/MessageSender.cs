using Dalamud.Plugin.Ipc;
using MidiBard.IPC.Internal.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.IPC.Internal.MessageSenders.Playback
{
    public class MessageSender : IPC.Internal.MessageSenders.MessageSender<PlaybackMessage>
    {

        public MessageSender(ICallGateProvider<MidibardMessage, MidibardMessage> provider) : base(provider)
        {
   
        }

        public void ChangeTempo(double speed)
        {
            Send(
                new PlaybackMessage()
                {
                    Type = PlaybackMessageType.ChangeTempo,
                    Data = new object[] { speed }
                },
            MidibardMessageType.Playback);
        }

        public void ChangeTime(int time)
        {
            Send(
                new PlaybackMessage()
                {
                    Type = PlaybackMessageType.ChangeTime,
                    Data = new object[] { time }
                },
            MidibardMessageType.Playback);
        }
    }
}
