using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.IPC.Internal.Messages
{
    public enum MidibardMessageType { Playback }

    [Serializable]
    public class MidibardMessage
    {
        public MidibardMessageType Type { get; set; }

        public object Message { get; set; }
    }
}
