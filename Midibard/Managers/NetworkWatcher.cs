using Dalamud.Logging;
using MidiBard.DalamudApi;
using System;
using System.Runtime.InteropServices;

namespace MidiBard.Managers
{
    internal class NetworkWatcher : IDisposable
    {
        public static event EventHandler<long> NetEnsembleCheckRequested;
        public static event EventHandler<long> NetEnsembleCheckFailed;
        public static event EventHandler<long> NetEnsembleStart;
        public static event EventHandler<long> NetEnsembleStop;

        public NetworkWatcher()
        {
            api.GameNetwork.NetworkMessage += GameNetwork_NetworkMessage;
        }

        private static bool ValidTimeSig(byte timeSig) => timeSig > 1 && timeSig < 8;
        private static bool ValidTempo(byte tempo) => tempo > 29 && tempo < 201;

        private void GameNetwork_NetworkMessage(IntPtr dataPtr, ushort opCode, uint sourceActorId, uint targetActorId, Dalamud.Game.Network.NetworkMessageDirection direction)
        {
            if (direction == Dalamud.Game.Network.NetworkMessageDirection.ZoneUp)
                return;

            //Get the timestamp
            byte[] msg = new byte[5];
            Marshal.Copy(dataPtr - 8, msg, 0, 4);
            long timeStamp = BitConverter.ToUInt32(msg, 0);
            timeStamp *= 1000;
            

            string hexString = BitConverter.ToString(msg);
            PluginLog.Debug(timeStamp.ToString());
            PluginLog.Debug("NET: " + Convert.ToString(opCode));
            PluginLog.Debug(hexString);

            switch (opCode)
            {
                case 201://EnsCheck request
                    byte[] message = new byte[25];
                    Marshal.Copy(dataPtr, message, 0, 24);
                    if (BitConverter.ToUInt16(message, 16) == 0 && ValidTempo(message[18]) && ValidTimeSig(message[19])) // 00 00 [tempo] [timesig]
                    {
                        NetEnsembleCheckRequested?.Invoke(this, timeStamp);
                        PluginLog.Debug("NET: Encheck request");
                    }
                    break;
                case 476: //EnsCheck
                    message = new byte[25];
                    Marshal.Copy(dataPtr, message, 0, 24);
                    uint reply = message[16];
                    if (reply > 2)
                        break;

                    if (message[16] == 0)
                        PluginLog.Debug("NET: Encheck No instrument");
                    if (message[16] == 01)
                        PluginLog.Debug("NET: Encheck ready"); //signal from the actors in grp
                    if (message[16] == 02)
                    {
                        NetEnsembleCheckFailed?.Invoke(this, timeStamp);
                        PluginLog.Debug("NET: Encheck failed");
                    }
                    break;
                case 584:
                    message = new byte[57];
                    Marshal.Copy(dataPtr, message, 0, 56);
                    //18 Tempo, 19 sig
                    if (
                        !(BitConverter.ToUInt16(message, 16) == 0 && ValidTempo(message[18]) &&
                          ValidTimeSig(message[19])) ||
                        BitConverter.ToUInt32(message, 12) > 0 || // These should all be zero in an ensemble start packet.
                        BitConverter.ToUInt32(message, 20) > 0 ||
                        BitConverter.ToUInt32(message, 24) > 0 ||
                        BitConverter.ToUInt32(message, 28) > 0 ||
                        BitConverter.ToUInt32(message, 32) > 0 ||
                        BitConverter.ToUInt32(message, 36) > 0 ||
                        BitConverter.ToUInt32(message, 40) > 0 ||
                        BitConverter.ToUInt32(message, 44) > 0
                    )
                        return;
                    NetEnsembleStart?.Invoke(this, timeStamp);
                    PluginLog.Debug("NET: Ens Start " + timeStamp.ToString());
                    break;
                case 889:
                    message = new byte[17];
                    Marshal.Copy(dataPtr, message, 0, 16);
                    if (BitConverter.ToUInt32(message, 12) != 0)
                        return;
                    NetEnsembleStop?.Invoke(this, timeStamp);
                    PluginLog.Debug("NET: Ens Stop " + timeStamp.ToString());
                    break;
            }
        }

        public void Dispose()
        {
            api.GameNetwork.NetworkMessage -= GameNetwork_NetworkMessage;
            NetEnsembleCheckRequested = delegate { };
            NetEnsembleCheckFailed = delegate { };
            NetEnsembleStart = delegate { };
            NetEnsembleStop = delegate { };
        }
    }
}