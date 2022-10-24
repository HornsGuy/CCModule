using ClientServerShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
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

        private List<int> GetPlayersTeamTropIndeces(MissionPeer playerPeer)
        {
            List<int> toReturn = new List<int>();
            foreach (var peer in GameNetwork.NetworkPeers)
            {
                MissionPeer teamMatePeer = peer.GetComponent<MissionPeer>();
                if(teamMatePeer != null)
                {
                    if(teamMatePeer.Team.Side == playerPeer.Team.Side)
                    {
                        toReturn.Add(teamMatePeer.SelectedTroopIndex);
                    }
                }
            }
            return toReturn;
        }

        public bool CheckIfPlayerTroopIndexIsUnderCap(MissionPeer playerPeer)
        {
            Dictionary<string, int> troopTypePercent = new Dictionary<string, int>();
            troopTypePercent.Add(new TextObject("{=1Bm1Wk1v}Infantry").ToString(), AdminPanelData.Instance.InfantryCap);
            troopTypePercent.Add(new TextObject("{=rangedtroop}Ranged").ToString(), AdminPanelData.Instance.RangedCap);
            troopTypePercent.Add(new TextObject("{=YVGtcLHF}Cavalry").ToString(), AdminPanelData.Instance.CavalryCap);

            Dictionary<int, string> typeDictionaryForPlayerTeam = playerPeer.Team.Side == BattleSideEnum.Attacker ? troopIndexToTypeTeam1 : troopIndexToTypeTeam2;
            Dictionary<string, float> currentTroopBreakdown = TroopCapLogic.GetCurrentTeamClassTypeBreakdown(GetPlayersTeamTropIndeces(playerPeer), typeDictionaryForPlayerTeam, troopTypePercent.Keys.ToList());

            Dictionary<string, bool> classIsAvailable = TroopCapLogic.GetTroopClassAvailabilityDictionary(currentTroopBreakdown, troopTypePercent);
            
            return classIsAvailable[typeDictionaryForPlayerTeam[playerPeer.SelectedTroopIndex]];
        }
    }
}
