using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromClient
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromClient)]
    public sealed class RequestMapsForGameType : GameNetworkMessage
    {

        public string GameType { get; set; }

        public RequestMapsForGameType(string gameType)
        {
            GameType = gameType;
        }

        public RequestMapsForGameType()
        {
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            GameType = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteStringToPacket(GameType);
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Request Maps For a game type";
    }
}
