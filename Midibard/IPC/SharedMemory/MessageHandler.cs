using MidiBard.Common.IPC;
using System;
using System.Collections.Generic;
using System.Text;

namespace MidiBard.IPC.SharedMemory
{
    public enum MessageType { ReloadPlaylist = 1, ReloadPlaylistSettings = 2, ChangeSong = 3, SwitchInstruments = 4, Restart = 5, Close = 6 }

    public class MessageHandler
    {

        public MessageHandler() 
        {

        }

        public event EventHandler ReloadPlaylistMessageReceived;

        public event EventHandler ReloadPlaylistSettingsMessageReceived;

        public event EventHandler<int> ChangeSongMessageReceived;

        public event EventHandler SwitchInstrumentsMessageReceived;

        public event EventHandler RestartHscmOverrideMessageReceived;

        public event EventHandler ClosePerformanceMessageReceived;

        public void HandleMessage(MessageType type, int args)
        {

            switch (type)
            {

                case MessageType.ReloadPlaylist:
                    ReloadPlaylistMessageReceived.Invoke(this, EventArgs.Empty);
                    break;

                case MessageType.ReloadPlaylistSettings:
                    ReloadPlaylistSettingsMessageReceived.Invoke(this, EventArgs.Empty);
                    break;

                case MessageType.ChangeSong:
                    ChangeSongMessageReceived.Invoke(this, args);
                    break;

                case MessageType.SwitchInstruments:
                    SwitchInstrumentsMessageReceived.Invoke(this, EventArgs.Empty);
                    break;

                case MessageType.Restart:
                    RestartHscmOverrideMessageReceived.Invoke(this, EventArgs.Empty);
                    break;

                case MessageType.Close:
                    ClosePerformanceMessageReceived.Invoke(this, EventArgs.Empty);
                    break;
            }
        }
    }
}
