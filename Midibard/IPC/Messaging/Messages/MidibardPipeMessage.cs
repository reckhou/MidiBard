using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.IPC.Messaging.Messages
{
    public enum MidibardPipeMessageType { Playback, Playlist }

    public class MidibardPipeMessage
    {
        public MidibardPipeMessageType Type { get; set; }

        public object Message { get; set; }
    }
}
