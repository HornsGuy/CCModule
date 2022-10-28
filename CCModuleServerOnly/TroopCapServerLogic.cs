using ClientServerShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
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

        private List<int> GetPlayersTeamTroopIndeces(MissionPeer playerPeer)
        {
            List<int> toReturn = new List<int>();
            foreach (var peer in GameNetwork.NetworkPeers)
            {
                MissionPeer teamMatePeer = peer.GetComponent<MissionPeer>();
                if(teamMatePeer != null && teamMatePeer.Team != null && playerPeer.Team != null)
                {
                    if(teamMatePeer.Team.Side == playerPeer.Team.Side)
                    {
                        toReturn.Add(teamMatePeer.SelectedTroopIndex);
                    }
                }
            }
            return toReturn;
        }

        public bool CheckIfPlayerTroopIndexIsUnderCap(MissionPeer playerPeer,int troopIndex)
        {
            Dictionary<string, int> troopTypePercent = new Dictionary<string, int>();
            troopTypePercent.Add(new TextObject("{=1Bm1Wk1v}Infantry").ToString(), AdminPanelData.Instance.InfantryCap);
            troopTypePercent.Add(new TextObject("{=rangedtroop}Ranged").ToString(), AdminPanelData.Instance.RangedCap);
            troopTypePercent.Add(new TextObject("{=YVGtcLHF}Cavalry").ToString(), AdminPanelData.Instance.CavalryCap);
            troopTypePercent.Add(new TextObject("{=ugJfuabA}Horse Archer").ToString(), AdminPanelData.Instance.HorseArcherCap);

            Dictionary<int, string> typeDictionaryForPlayerTeam = playerPeer.Culture.ToString().ToLower() == MultiplayerOptions.OptionType.CultureTeam1.GetStrValue().ToLower() ? troopIndexToTypeTeam1 : troopIndexToTypeTeam2;
            Dictionary<string, float> currentTroopBreakdown = TroopCapLogic.GetCurrentTeamClassTypeBreakdown(GetPlayersTeamTroopIndeces(playerPeer), typeDictionaryForPlayerTeam, troopTypePercent.Keys.ToList());

            Dictionary<string, bool> classIsAvailable = TroopCapLogic.GetTroopClassAvailabilityDictionary(currentTroopBreakdown, troopTypePercent);

            //Debug.Print(troopIndex.ToString(), 0,Debug.DebugColor.Magenta);
            //Debug.Print(typeDictionaryForPlayerTeam[troopIndex], 0,Debug.DebugColor.Magenta);
            //Debug.Print(classIsAvailable[typeDictionaryForPlayerTeam[troopIndex]].ToString(), 0,Debug.DebugColor.Magenta);
            //Debug.Print(currentTroopBreakdown[typeDictionaryForPlayerTeam[troopIndex]].ToString(), 0,Debug.DebugColor.Magenta);
            //Debug.Print(AdminPanelData.Instance.InfantryCap.ToString(), 0,Debug.DebugColor.Magenta);

            return classIsAvailable[typeDictionaryForPlayerTeam[troopIndex]];
        }
    }
}
