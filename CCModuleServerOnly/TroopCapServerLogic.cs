using BannerlordWrapper;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace CCModuleServerOnly
{
    class TroopCapServerLogic
    {
        static TroopCapServerLogic _instance;
        public static TroopCapServerLogic Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new TroopCapServerLogic();
                }
                return _instance;
            }
        }

        public bool CheckIfPlayerTroopIndexIsUnderCap(string ID, int troopIndex)
        {
            BannerlordWrapper.Team playerTeam = PlayerWrapper.Instance.GetPlayer(ID).Team;
            int troopCapPercent = AdminPanelServerData.Instance.GetTroopCapForTroopType(playerTeam.GetTroopType(troopIndex));
            return TroopCapLogic.TroopUnderCapForTeam(playerTeam, troopIndex, troopCapPercent) || playerTeam.TeamType == TeamType.Spectator;
        }

        public void OnTroopCapChange()
        {
            foreach (var netPeer in GameNetwork.NetworkPeers)
            {
                MissionPeer mp = netPeer.GetComponent<MissionPeer>();
                if(mp != null)
                {
                    if(!CheckIfPlayerTroopIndexIsUnderCap(netPeer.VirtualPlayer.Id.ToString(), mp.SelectedTroopIndex))
                    {
                        mp.SelectedTroopIndex = 0;

                        BannerlordWrapper.PlayerWrapper.Instance.SetTroopIndexForPlayer(netPeer.VirtualPlayer.Id.ToString(), 0);
                        
                        GameNetwork.BeginBroadcastModuleEvent();
                        GameNetwork.WriteMessage((GameNetworkMessage)new UpdateSelectedTroopIndex(netPeer, 0));
                        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, netPeer);

                        if(mp.HasSpawnedAgentVisuals)
                        {
                            AgentBuildData abd = AdminPanel.Instance.GetAgentBuildDataForPlayer(netPeer).Item1;
                            GameNetwork.BeginModuleEventAsServer(netPeer);
                            GameNetwork.WriteMessage((GameNetworkMessage)new CreateAgentVisuals(netPeer, abd, mp.SelectedTroopIndex, 0));
                            GameNetwork.EndModuleEventAsServer();
                        }

                    }
                }
            }
        }
    }
}
