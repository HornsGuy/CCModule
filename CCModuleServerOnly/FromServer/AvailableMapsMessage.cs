using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class AvailableMapsMessage : GameNetworkMessage
    {

        public List<string> maps { get; private set; }

        public AvailableMapsMessage(List<string> maps)
        {
            this.maps = maps;
        }

        public AvailableMapsMessage()
        {
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            string temp = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
            if(temp.Contains(" "))
            {
                maps = new List<string>(temp.Split(' '));
            }

            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            string temp = string.Join(" ", maps);
            GameNetworkMessage.WriteStringToPacket(temp);
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Contains list of maps";
    }
}
