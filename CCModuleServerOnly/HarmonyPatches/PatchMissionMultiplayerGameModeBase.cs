using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace CCModuleServerOnly.HarmonyPatches
{
    class PatchMissionMultiplayerGameModeBase
    {
        public static bool Prefix(MissionMultiplayerGameModeBase __instance, MissionPeer peer, ref int newAmount)
        {
            if (peer != null)
            {
                NetworkCommunicator netPeer = peer.GetNetworkPeer();
                if (netPeer != null && netPeer.VirtualPlayer != null && netPeer.VirtualPlayer.Id != null)
                {
                    string ID = netPeer.VirtualPlayer.Id.ToString();
                    if (EquipmentOverride.Instance.PlayerHasEquipmentToBeOverridden(ID))
                    {
                        newAmount = 300;
                    }
                }
            }
            return true;
        }
    }
}
