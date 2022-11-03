using BannerlordWrapper;
using CCModuleNetworkMessages.FromServer;
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
            
            if (peer != null && peer.VirtualPlayer != null && peer.VirtualPlayer.Id != null)
            {
                if (!TroopCapServerLogic.Instance.CheckIfPlayerTroopIndexIsUnderCap(peer.VirtualPlayer.Id.ToString(), message.SelectedTroopIndex))
                {
                    Logging.Instance.Info($"Player with ID {peer.VirtualPlayer.Id} was denied troop ID {message.SelectedTroopIndex}");
                    GameNetwork.BeginModuleEventAsServer(peer);
                    GameNetwork.WriteMessage(new LargeTextServerMessage("Over Troop Cap, Select A Different Class"));
                    GameNetwork.EndModuleEventAsServer();
                    return false;
                }
                else
                {
                    GameNetwork.BeginModuleEventAsServer(peer);
                    GameNetwork.WriteMessage(new LargeTextServerMessage(""));
                    GameNetwork.EndModuleEventAsServer();
                }
            }
            PlayerWrapper.Instance.SetTroopIndexForPlayer(peer.VirtualPlayer.Id.ToString(), message.SelectedTroopIndex);
            return true;
        }
    }
}
