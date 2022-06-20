using MidiBard.Common.Messaging.Messages;
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

        public event DataReceivedEventHandler<T> DataReceived;

        protected NamedPipe(string name)
        {

        }

        protected void Stop()
        {
            stream.Close();
        }

        public void Send(T msg)
        {
            var data = BinarySerializer.Serialize(msg);

            if (data == null)
                return;

            writeLen(data.Length);
            stream.Flush();
            writeObj(msg);
            stream.Flush();
            //stream.WaitForPipeDrain(); ;
            //stream.WaitForPipeDrain();
        }


        private void writeObj(T obj)
        {
            var data = BinarySerializer.Serialize(obj);

            if (data == null)
                return;

            stream.Write(data, 0, data.Length);
        }

        private void writeLen(int len)
        {
            var lenbuf = BitConverter.GetBytes(len);
            stream.Write(lenbuf, 0, lenbuf.Length);
        }

        private T readObj(int len)
        {
            var buffer = new byte[len];

            int bytesRead = stream.Read(buffer, 0, len);

            if (bytesRead == 0)
                return null;

            var obj = BinarySerializer.Deserialize<T>(buffer);
            return obj as T;
        }
        private int readLen()
        {
            const int lensize = sizeof(int);
            var lenbuf = new byte[lensize];
            var bytesRead = stream.Read(lenbuf, 0, lensize);

            if (bytesRead == 0)
                return 0;

            if (bytesRead != lensize)
                return 0;

            return BitConverter.ToInt32(lenbuf, 0);
        }

        protected void StartPipeThread()
        {

            Task.Run(() =>
            {
                while (stream.IsConnected)
                {
                    int len = readLen();

                    if (len == 0)
                        stream.Close();

                    var obj = readObj(len);

                    if (obj == null)
                        stream.Close();

                    if (DataReceived != null)
                        DataReceived.Invoke(this, obj);

                    //var okMsg = new MidibardPipeMessage() { Type = MidibardPipeMessageType.OK };

                    //Send(okMsg as T);

                }
            });
        }

        public abstract void Start();

    }
}
