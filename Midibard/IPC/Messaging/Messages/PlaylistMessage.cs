using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.IPC.Messaging.Messages
{
    public enum PlaylistMessageType { Reload }

    public class PlaylistMessage
    {
        public PlaylistMessageType Type { get; set; }


        public object[] Data { get; set; }
    }
}
