using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class ReEquipInitialWeapons : GameNetworkMessage
    {

        public ReEquipInitialWeapons()
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

        protected override string OnGetLogFormat() => "";
    }
}
