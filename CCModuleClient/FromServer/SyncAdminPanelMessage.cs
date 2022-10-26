using System.Collections.Generic;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class SyncAdminPanelMessage : GameNetworkMessage
    {

        public int InfantryCap { get; set; }
        public int RangedCap { get; set; }
        public int CavalryCap { get; set; }
        public int HorseArcherCap { get; set; }
        public bool PrintMessage { get; set; }

        public List<string> AvailableMaps { get; set; }

        public void SetTroopCaps(int infCap, int rangeCap, int cavCap, int haCap)
        {
            InfantryCap = infCap;
            RangedCap = rangeCap;
            CavalryCap = cavCap;
            HorseArcherCap = haCap;
        }

        public SyncAdminPanelMessage(bool printMessage)
        {
            PrintMessage = printMessage;
        }

        public SyncAdminPanelMessage()
        {
        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;

            // Troop Cap
            InfantryCap = GameNetworkMessage.ReadIntFromPacket(new CompressionInfo.Integer(0,8), ref bufferReadValid);
            RangedCap = GameNetworkMessage.ReadIntFromPacket(new CompressionInfo.Integer(0, 8), ref bufferReadValid);
            CavalryCap = GameNetworkMessage.ReadIntFromPacket(new CompressionInfo.Integer(0, 8), ref bufferReadValid);
            HorseArcherCap = GameNetworkMessage.ReadIntFromPacket(new CompressionInfo.Integer(0, 8), ref bufferReadValid);
            PrintMessage = GameNetworkMessage.ReadBoolFromPacket(ref bufferReadValid);

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
            // Troop Cap
            GameNetworkMessage.WriteIntToPacket(InfantryCap, new CompressionInfo.Integer(0, 8));
            GameNetworkMessage.WriteIntToPacket(RangedCap, new CompressionInfo.Integer(0, 8));
            GameNetworkMessage.WriteIntToPacket(CavalryCap, new CompressionInfo.Integer(0, 8));
            GameNetworkMessage.WriteIntToPacket(HorseArcherCap, new CompressionInfo.Integer(0, 8));
            GameNetworkMessage.WriteBoolToPacket(PrintMessage);

            // Maps
            string temp = string.Join(" ", AvailableMaps);
            GameNetworkMessage.WriteStringToPacket(temp);

        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Tell client what the current admin panel data is";
    }
}
