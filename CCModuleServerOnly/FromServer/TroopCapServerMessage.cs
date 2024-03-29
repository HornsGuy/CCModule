﻿using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class TroopCapServerMessage : GameNetworkMessage
    {

        public int InfantryCap { get; set; }
        public int RangedCap { get; set; }
        public int CavalryCap { get; set; }
        public int HorseArcherCap { get; set; }
        public bool PrintMessage { get; set; }

        public TroopCapServerMessage(int infCap, int rangeCap, int cavCap, int haCap, bool printMessage)
        {
            InfantryCap = infCap;
            RangedCap = rangeCap;
            CavalryCap = cavCap;
            HorseArcherCap = haCap;
            PrintMessage = printMessage;
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
            HorseArcherCap = GameNetworkMessage.ReadIntFromPacket(CompressionBasic.DebugIntNonCompressionInfo, ref bufferReadValid);
            PrintMessage = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);

            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteIntToPacket(InfantryCap, CompressionBasic.DebugIntNonCompressionInfo);
            GameNetworkMessage.WriteIntToPacket(RangedCap, CompressionBasic.DebugIntNonCompressionInfo);
            GameNetworkMessage.WriteIntToPacket(CavalryCap, CompressionBasic.DebugIntNonCompressionInfo);
            GameNetworkMessage.WriteIntToPacket(HorseArcherCap, CompressionBasic.DebugIntNonCompressionInfo);
            GameNetworkMessage.WriteBoolToPacket(PrintMessage);
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Tell client what the new troop caps are";
    }
}
