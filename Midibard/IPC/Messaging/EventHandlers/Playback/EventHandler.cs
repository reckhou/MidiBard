using MidiBard.Control.MidiControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard;

public partial class MidiBard
{
    private static void PlaybackMessageHandler_PlayMessageReceived(object sender, int index)
    {
        MidiPlayerControl.SwitchSong(index);
    }
}
