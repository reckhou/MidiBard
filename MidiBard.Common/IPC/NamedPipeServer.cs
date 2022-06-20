using MidiBard.Common.IPC;
using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.Common.IPC
{
    public class NamedPipeServer<T> : NamedPipe<T> where T : class
    {
        public NamedPipeServer(string name) : base(name)
        {
            //PipeSecurity ps = new PipeSecurity();
            //ps.AddAccessRule(new PipeAccessRule("Users", PipeAccessRights.ReadWrite | PipeAccessRights.CreateNewInstance, AccessControlType.Allow));
            //ps.AddAccessRule(new PipeAccessRule("CREATOR OWNER", PipeAccessRights.FullControl, AccessControlType.Allow));
            //ps.AddAccessRule(new PipeAccessRule("SYSTEM", PipeAccessRights.FullControl, AccessControlType.Allow));

            PipeSecurity pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule("Everyone",
            PipeAccessRights.ReadWrite, AccessControlType.Allow));

                stream = new NamedPipeServerStream(name, PipeDirection.InOut, 8, PipeTransmissionMode.Message, PipeOptions.Asynchronous, 1024, 1024, pipeSecurity);
        }

        public override void Start()
        {
            ((NamedPipeServerStream)stream).WaitForConnection();

            StartPipeThread();
        }
    }
}
