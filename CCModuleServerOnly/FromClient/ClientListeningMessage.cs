using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class ClientListeningMessage : GameNetworkMessage
    {

        public ClientListeningMessage()
        {
        }

        protected override bool OnRead()
        {
            return true;
        }

        protected override void OnWrite()
        {
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Message from the client to tell the server they are now listening for server events";
    }
}
