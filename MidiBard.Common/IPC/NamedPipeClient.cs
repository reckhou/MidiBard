using MidiBard.Common.Messaging.Messages;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.Common.IPC
{
    public class NamedPipeClient<T> : NamedPipe<T> where T : class
    {
        public NamedPipeClient(string name) : base(name)
        {
            stream = new NamedPipeClientStream(".", name, PipeDirection.InOut, PipeOptions.Asynchronous);
        }

        public override void Start()
        {
            ((NamedPipeClientStream)stream).Connect();
          
            StartPipeThread();

            //var okMsg = new MidibardPipeMessage() { Type = MidibardPipeMessageType.OK };

            //Send(okMsg as T);

        }
    }
}
