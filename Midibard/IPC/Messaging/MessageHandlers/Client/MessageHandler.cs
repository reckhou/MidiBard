using MidiBard.IPC.Messaging.Messages;
using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midibard.IPC.Messaging.Handlers.Client
{
    public abstract class MessageHandler<T> where T : class
    {
        private readonly NamedPipeClient<MidibardPipeMessage> client;

        protected MessageHandler(NamedPipeClient<MidibardPipeMessage> client)
        {
            this.client = client;
            this.client.ServerMessage += OnMessageReceived;
        }

        protected void OnMessageReceived(object sender, MidibardPipeMessage msg)
        {
            HandleMessage(msg.Message as T);
        }

        protected abstract void HandleMessage(T msg);
    }
}
