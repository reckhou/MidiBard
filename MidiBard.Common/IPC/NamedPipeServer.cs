using MidiBard.Common.IPC;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.Common.IPC
{
    public class NamedPipeServer<T> : NamedPipe<T> where T : class
    {
        public NamedPipeServer(string name) : base(name)
        {
            stream = new NamedPipeServerStream(name, PipeDirection.InOut, 8, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
        }

        public override void Start()
        {
            ((NamedPipeServerStream)stream).WaitForConnection();

            StartPipeThread();
        }
    }
}
