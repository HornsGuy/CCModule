using ClientServerShared;
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
        Dictionary<int, string> troopIndexToTypeTeam1 = new Dictionary<int, string>();
        Dictionary<int, string> troopIndexToTypeTeam2 = new Dictionary<int, string>();

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

        public void Setup()
        {
            troopIndexToTypeTeam1 = PopulateTroopIndexToTypeDictionary(MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue())).ToList());
            troopIndexToTypeTeam2 = PopulateTroopIndexToTypeDictionary(MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue())).ToList());
        }

        private Dictionary<int, string> PopulateTroopIndexToTypeDictionary(List<MultiplayerClassDivisions.MPHeroClass> classes)
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>();
            int index = 0;
            foreach (var troopClass in classes)
            {
                toReturn.Add(index,troopClass.ClassGroup.Name.ToString());
                index++;
            }

            return toReturn;
        }


        public bool CheckIfPlayerTroopIndexIsUnderCap(MissionPeer playerPeer, int troopIndex)
        {
            Dictionary<string, int> troopTypePercent = new Dictionary<string, int>();
            troopTypePercent.Add(new TextObject("{=1Bm1Wk1v}Infantry").ToString(), AdminPanelServerData.Instance.InfantryCap);
            troopTypePercent.Add(new TextObject("{=rangedtroop}Ranged").ToString(), AdminPanelServerData.Instance.RangedCap);
            troopTypePercent.Add(new TextObject("{=YVGtcLHF}Cavalry").ToString(), AdminPanelServerData.Instance.CavalryCap);
            troopTypePercent.Add(new TextObject("{=ugJfuabA}Horse Archer").ToString(), AdminPanelServerData.Instance.HorseArcherCap);

            Dictionary<int, string> typeDictionaryForPlayerTeam = playerPeer.Culture.ToString().ToLower() == MultiplayerOptions.OptionType.CultureTeam1.GetStrValue().ToLower() ? troopIndexToTypeTeam1 : troopIndexToTypeTeam2;
            List<int> troopIndeces = PlayerWrapper.GetPeerTeamTroopIndeces(playerPeer, true);

            Dictionary<string, float> currentTroopBreakdown = TroopCapLogic.GetCurrentTeamClassTypeBreakdown(troopIndeces, typeDictionaryForPlayerTeam, troopTypePercent.Keys.ToList());
            Dictionary<string, bool> classIsAvailable = TroopCapLogic.GetTroopClassAvailabilityDictionary(currentTroopBreakdown, troopTypePercent);

            return classIsAvailable[typeDictionaryForPlayerTeam[troopIndex]];
        }

        public void OnTroopCapChange()
        {
            foreach (var netPeer in GameNetwork.NetworkPeers)
            {
                MissionPeer mp = netPeer.GetComponent<MissionPeer>();
                if(mp != null)
                {
                    if(!CheckIfPlayerTroopIndexIsUnderCap(mp, mp.SelectedTroopIndex))
                    {
                        mp.SelectedTroopIndex = 0;
                        GameNetwork.BeginBroadcastModuleEvent();
                        GameNetwork.WriteMessage((GameNetworkMessage)new UpdateSelectedTroopIndex(netPeer, 0));
                        GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, netPeer);

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
