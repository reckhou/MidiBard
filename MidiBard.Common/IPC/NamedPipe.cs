using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.Common.IPC
{
    public abstract class NamedPipe<T> where T : class
    {
        public delegate void DataReceivedEventHandler<TRead>(NamedPipe<T> pipe, TRead message);

        protected PipeStream stream;

        private bool stopped = false;

        public event DataReceivedEventHandler<T> DataReceived;

        protected NamedPipe(string name)
        {
            
        }

        protected void Stop()
        {
            stopped = true;
        }

        protected void Send(T msg)
        {
            var data = BinarySerializer.Serialize(msg);
            if (data != null)
                stream.Write(data, 0, data.Length);
        }

        protected void StartPipeThread()
        {

            Task.Run(() =>
            {
                while (!stopped)
                {
                    var buffer = new byte[4096];
                    int total = stream.Read(buffer, 0, 4096);
                    if (total > 0)
                    {
                        var msg = BinarySerializer.Deserialize<T>(buffer);
                        if (msg != null)
                            DataReceived(this, msg);
                    }
                }
            });
        }

        public abstract void Start();

    }
}
