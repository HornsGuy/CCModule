using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class AdminLoginMessage : GameNetworkMessage
    {

        public AdminLoginMessage()
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

        protected override string OnGetLogFormat() => "Telling client they have admin perms";
    }
}
