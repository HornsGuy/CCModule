using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace CCModuleNetworkMessages.FromServer
{
    [DefineGameNetworkMessageTypeForMod(GameNetworkMessageSendType.FromServer)]
    public sealed class ChangePlayerCosmeticEquipment : GameNetworkMessage
    {
        public NetworkCommunicator Peer;
        public Equipment Equipment { get; set; }


        public ChangePlayerCosmeticEquipment(NetworkCommunicator peer, Equipment equip)
        {
            Peer = peer;
            Equipment = equip;
        }

        public ChangePlayerCosmeticEquipment()
        {

        }

        protected override bool OnRead()
        {
            bool bufferReadValid = true;

            Peer = GameNetworkMessage.ReadNetworkPeerReferenceFromPacket(ref bufferReadValid);
            Equipment = new Equipment();
            for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; ++equipmentIndex)
                Equipment.AddEquipmentToSlotWithoutAgent(equipmentIndex, ModuleNetworkData.ReadItemReferenceFromPacket(MBObjectManager.Instance, ref bufferReadValid));

            return bufferReadValid;
        }

        protected override void OnWrite()
        {
            GameNetworkMessage.WriteNetworkPeerReferenceToPacket(Peer);
            for (EquipmentIndex equipmentIndex = EquipmentIndex.WeaponItemBeginSlot; equipmentIndex < EquipmentIndex.NumEquipmentSetSlots; ++equipmentIndex)
                ModuleNetworkData.WriteItemReferenceToPacket(Equipment.GetEquipmentFromSlot(equipmentIndex));
        }

        protected override MultiplayerMessageFilter OnGetLogFilter() => MultiplayerMessageFilter.Mission;

        protected override string OnGetLogFormat() => "Add a message to the clinets chat log with the given color";
    }
}
