using Dalamud.Plugin.Ipc;
using MidiBard.IPC.Internal.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.IPC.Internal.MessageSenders
{

    public abstract class MessageSender<T>
    {
        private ICallGateProvider<MidibardMessage, MidibardMessage> provider;

        public MessageSender(ICallGateProvider<MidibardMessage, MidibardMessage> provider)
        {
            this.provider = provider;
        }


        protected void Send(T msg, MidibardMessageType type)
        {

            this.provider.SendMessage(new MidibardMessage()
            {
                Type = type,
                Message = msg
            });

        }
    }
}
