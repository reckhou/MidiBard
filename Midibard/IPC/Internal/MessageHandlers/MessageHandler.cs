
using Dalamud.Plugin.Ipc;
using MidiBard.Common.IPC;
using MidiBard.Common.Messaging.Messages;
using MidiBard.IPC.Internal.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.IPC.Internal.MessageHandlers
{
    public abstract class MessageHandler<T> where T : class
    {
        private readonly ICallGateSubscriber<MidibardMessage, MidibardMessage> subscriber;

        public MessageHandler(ICallGateSubscriber<MidibardMessage, MidibardMessage> subscriber)
        {
            this.subscriber = subscriber;
            this.subscriber.Subscribe(OnMessageReceived);
        }

        protected void OnMessageReceived(MidibardMessage msg)
        {
            HandleMessage(msg.Message as T);
        }

        protected abstract void HandleMessage(T msg);
    }
}
