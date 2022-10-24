using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class TroopCapServerMessage : GameNetworkMessage
    {

        public int InfantryCap { get; set; }
        public int RangedCap { get; set; }
        public int CavalryCap { get; set; }

        public TroopCapServerMessage(int infCap, int rangeCap, int cavCap)
        {
            InfantryCap = infCap;
            RangedCap = rangeCap;
            CavalryCap = cavCap;
        }

        public TroopCapServerMessage()
        {
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;
            InfantryCap = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);
            RangedCap = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);
            CavalryCap = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);

            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteIntToPacket(InfantryCap, CompressionBasic.DebugIntNonCompressionInfo);
            GameNetworkMessage.WriteIntToPacket(RangedCap, CompressionBasic.DebugIntNonCompressionInfo);
            GameNetworkMessage.WriteIntToPacket(CavalryCap, CompressionBasic.DebugIntNonCompressionInfo);
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Tell client what the new troop caps are";
    }
}
