using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.IPC.Messaging.Messages
{
    public enum PlaybackMessageType { Play, Stop, Pause }

    public class PlaybackMessage
    {
        public PlaybackMessageType Type { get; set; }


        public object[] Data { get; set; }
    }
}
