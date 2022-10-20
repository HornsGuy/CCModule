using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class APEndWarmupMessage : GameNetworkMessage
    {
        public NetworkCommunicator PlayerPeer { get; private set; }

        public APEndWarmupMessage(NetworkCommunicator playerPeer)
        {
            this.PlayerPeer = playerPeer;
        }

        public APEndWarmupMessage()
        {
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            this.PlayerPeer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteNetworkPeerReferenceToPacket(this.PlayerPeer);
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Client requested warmup to end via admin panel";
    }
}
