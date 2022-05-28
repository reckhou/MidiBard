using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard.IPC.Internal.Messages
{
    public enum PlaybackMessageType { ChangeTempo, ChangeTime }

    [Serializable]
    public class PlaybackMessage
    {
        public PlaybackMessageType Type { get; set; }


        public object[] Data { get; set; }
    }
}
