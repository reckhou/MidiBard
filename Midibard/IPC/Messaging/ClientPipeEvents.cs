using MidiBard.Common.Messaging.Messages;
using MidiBard.Control.MidiControl;
using NamedPipeWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard;

public partial class MidiBard
{
    private void ClientPipe_Disconnected(NamedPipeConnection<MidibardPipeMessage, MidibardPipeMessage> e)
    {
        Dalamud.Logging.PluginLog.Debug($"Disconnected from the IPC server.");
    }
}
