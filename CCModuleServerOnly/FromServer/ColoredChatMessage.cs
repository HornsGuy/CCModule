using System.Collections.Generic;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleNetworkMessages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class ColoredChatMessage : GameNetworkMessage
    {
        string Message { get; set; }
        float Red { get; set; }
        float Green { get; set; }
        float Blue { get; set; }

        static readonly CompressionInfo.Float compressionInfo = new CompressionInfo.Float(0.0f, 255.0f, 9);

        public ColoredChatMessage(string message, float red, float green, float blue)
        {
            Message = message;
            Red = red;
            Green = green;
            Blue = blue;
        }

        public ColoredChatMessage()
        {

        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;

            Message = GameNetworkMessage.ReadStringFromPacket(ref bufferReadValid);
            Red = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
            Green = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);
            Blue = GameNetworkMessage.ReadFloatFromPacket(compressionInfo, ref bufferReadValid);

            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteStringToPacket(Message);
            GameNetworkMessage.WriteFloatToPacket(Red, compressionInfo);
            GameNetworkMessage.WriteFloatToPacket(Green, compressionInfo);
            GameNetworkMessage.WriteFloatToPacket(Blue, compressionInfo);
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Add a message to the clinets chat log with the given color";
    }
}
