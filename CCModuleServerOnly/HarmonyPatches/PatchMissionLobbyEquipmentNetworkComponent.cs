using BannerlordWrapper;
using NetworkMessages.FromClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace CCModuleServerOnly.HarmonyPatches
{
    public class PatchMissionLobbyEquipmentNetworkComponent
    {
        public static bool Prefix(Mission __instance, NetworkCommunicator peer, RequestTroopIndexChange message)
        {
            Debug.Print("Patched lobby equipment", 0, Debug.DebugColor.Magenta);
            if (peer != null && peer.VirtualPlayer != null && peer.VirtualPlayer.Id != null)
            {
                if (!TroopCapServerLogic.Instance.CheckIfPlayerTroopIndexIsUnderCap(peer.VirtualPlayer.Id.ToString(), message.SelectedTroopIndex))
                {
                    FieldInfo fi = typeof(RequestTroopIndexChange).GetField("SelectedTroopIndex");
                    fi.SetValue(message, 0);
                    Debug.Print("Player Troop Changed!", 0, Debug.DebugColor.Magenta);
                }
            }
            PlayerWrapper.Instance.SetTroopIndexForPlayer(peer.VirtualPlayer.Id.ToString(), message.SelectedTroopIndex);
            return true;
        }
    }
}
