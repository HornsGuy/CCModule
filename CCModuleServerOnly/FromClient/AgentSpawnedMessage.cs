using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class AgentSpawnedMessage : GameNetworkMessage
    {

        public NetworkCommunicator peer;

        public AgentSpawnedMessage(NetworkCommunicator peer)
        {
            this.peer = peer;
        }

        public AgentSpawnedMessage()
        {
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteNetworkPeerReferenceToPacket(peer);
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Client letting server know a player agent has spawned";
    }
}
