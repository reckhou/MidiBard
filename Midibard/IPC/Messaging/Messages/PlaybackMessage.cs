using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Midibard.IPC.Messaging.Messages
{
    public enum PlaybackMessageType { Play, Stop, Pause }

    [Serializable]
    public class PlaybackMessage
    {
        public PlaybackMessageType Type { get; set; }


        public object[] Data { get; set; }
    }
}
