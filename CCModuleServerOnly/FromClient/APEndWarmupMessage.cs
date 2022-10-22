using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class APEndWarmupMessage : GameNetworkMessage
    {

        public APEndWarmupMessage()
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

        protected override string OnGetLogFormat() => "Client requested warmup to end via admin panel";
    }
}
