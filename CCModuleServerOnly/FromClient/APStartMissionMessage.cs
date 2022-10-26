using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class APStartMissionMessage : GameNetworkMessage
    {

        public string GameType { get; set; }
        public string Map { get; set; }
        public string Faction1 { get; set; }
        public string Faction2 { get; set; }

        public APStartMissionMessage(string gameType, string map, string faction1, string faction2)
        {
            GameType = gameType;
            Map = map;
            Faction1 = faction1;
            Faction2 = faction2;
        }

        public APStartMissionMessage()
        {
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            GameType = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
            Map = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
            Faction1 = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
            Faction2 = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteStringToPacket(GameType);
            GameNetworkMessage.WriteStringToPacket(Map);
            GameNetworkMessage.WriteStringToPacket(Faction1);
            GameNetworkMessage.WriteStringToPacket(Faction2);
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Starts the mission";
    }
}
