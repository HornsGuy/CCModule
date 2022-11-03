using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
namespace CCModuleServerOnly.HarmonyPatches
{
    class PatchMission
    {
        public static bool Prefix(Mission __instance, AgentBuildData agentBuildData, bool spawnFromAgentVisuals, int formationTroopCount)
        {
            MissionPeer curPeer = agentBuildData.AgentMissionPeer;
            if(curPeer != null)
            {
                NetworkCommunicator netPeer = curPeer.GetNetworkPeer();
                if(netPeer != null && netPeer.VirtualPlayer != null && netPeer.VirtualPlayer.Id != null)
                {
                    string ID = netPeer.VirtualPlayer.Id.ToString();
                    if(EquipmentOverride.Instance.PlayerHasEquipmentToBeOverridden(ID) && agentBuildData.AgentData != null && agentBuildData.AgentData.AgentOverridenEquipment != null)
                    {
                        Equipment newEquipment = EquipmentOverride.Instance.GetOverriddenEquipment(ID, agentBuildData.AgentData.AgentOverridenEquipment);
                        agentBuildData = agentBuildData.Equipment(newEquipment);
                        Debug.Print("Overridding spawn equipment",0,Debug.DebugColor.Magenta);
                    }
                }
            }
            return true;
        }
    }
}
