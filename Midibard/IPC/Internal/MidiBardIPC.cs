using Dalamud.Logging;
using Dalamud.Plugin.Ipc;
using MidiBard.IPC.Internal.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MidiBard
{
    public partial class MidiBard
    {

        const string ProviderName = "Midibard.ipc.from";

        const string SubscriberName = "Midibard.ipc.to";

        private static ICallGateProvider<MidibardMessage, MidibardMessage> provider;

        private static ICallGateSubscriber<MidibardMessage, MidibardMessage> subscriber;

        private static IPC.Internal.MessageHandlers.Playback.MessageHandler internalPlaybackMessageHandler;

        public static IPC.Internal.MessageSenders.Playback.MessageSender InternalPlaybackMessageSender;

        private static void InitInternalIPC()
        {
            PluginLog.Information($"Starting internal plugin IPC.");

            provider = DalamudApi.api.PluginInterface.GetIpcProvider<MidibardMessage, MidibardMessage>(ProviderName);
            subscriber = DalamudApi.api.PluginInterface.GetIpcSubscriber<MidibardMessage, MidibardMessage>(SubscriberName);
            internalPlaybackMessageHandler = new IPC.Internal.MessageHandlers.Playback.MessageHandler(subscriber);
            InternalPlaybackMessageSender = new IPC.Internal.MessageSenders.Playback.MessageSender(provider);
        }
    }
}
