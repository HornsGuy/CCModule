using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class LargeTextServerMessage : GameNetworkMessage
    {
        public string Message { get; set; }

        public LargeTextServerMessage(string message)
        {
            Message = message;
        }

        public LargeTextServerMessage()
        {

        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;

            Message = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);

            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteStringToPacket(Message);
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Displays large red text in the center of the client screen";
    }
}
