using Dalamud.Logging;
using MidiBard.HSC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard
{
    public static class SharedClientInfo
    {
        static List<ClientInfo> clients;

        const string SharedMemoryFileName = "MidiBard.ClientInfo";

        static SharedClientInfo()
        {
            clients = new List<ClientInfo>();
        }

        public static List<ClientInfo> Clients
        {
            get
            {
                byte[] data = null;

                try
                {
                    var sb = new SharedMemory.SharedArray<byte>(SharedMemoryFileName);
                    data = sb.ToArray();

                    if (data.IsNullOrEmpty())
                    {
                        clients = new List<ClientInfo>();
                        return clients;
                    }

                    clients = BinarySerializer.Deserialize<List<ClientInfo>>(data);
                }
                catch (Exception ex)
                {
                    //TODO log in plugin
                    PluginLog.Information($"HSC client info read in shared memory failed/n/rMessage: '{ex.Message}'");
                    clients = new List<ClientInfo>();
                }

                return clients;
            }
        }

        public static ClientInfo Add(string charName)
        {
            int index = clients.Count;

            var clientInfo = new ClientInfo() { CharName = charName, Index = index };
            clients.Add(clientInfo);

            try
            {
                var sb = new SharedMemory.SharedArray<byte>(SharedMemoryFileName);
                var data = BinarySerializer.Serialize(clients);
                sb.Write(data);
            }
            //TODO log in plugin
            catch (Exception ex)
            {
                PluginLog.Information($"HSC client info update in shared memory failed/n/rMessage: '{ex.Message}'");
                return clientInfo;
            }

            return clientInfo;
        }
    }
}
