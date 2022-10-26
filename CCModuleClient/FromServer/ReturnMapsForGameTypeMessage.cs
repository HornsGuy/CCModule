using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class ReturnMapsForGameTypeMessage : GameNetworkMessage
    {
        public List<string> AvailableMaps { get; set; }

        public ReturnMapsForGameTypeMessage(List<string> maps)
        {
            AvailableMaps = maps;
        }

        public ReturnMapsForGameTypeMessage()
        {

        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;

            // Maps
            string temp = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
            if (temp.Contains(" "))
            {
                AvailableMaps = new List<string>(temp.Split(' '));
            }

            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            // Maps
            string temp = string.Join(" ", AvailableMaps);
            GameNetworkMessage.WriteStringToPacket(temp);

        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Give client maps for selected game type";
    }
}
