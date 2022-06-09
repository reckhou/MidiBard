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
            stream = new NamedPipeClientStream(name);
        }

        public override void Start()
        {
            ((NamedPipeClientStream)stream).Connect();

            StartPipeThread();
        }
    }
}
