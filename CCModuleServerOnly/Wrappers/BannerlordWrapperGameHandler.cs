using NetworkMessages.FromClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade;
using BannerlordWrapper;
using TaleWorlds.ObjectSystem;
using TaleWorlds.Localization;
using TroopType = BannerlordWrapper.TroopType;

namespace CCModuleServerOnly.Wrappers
{
    public class BannerlordWrapperGameHandler : GameHandler
    {
        protected override void OnPlayerConnect(VirtualPlayer peer)
        {
            PlayerWrapper.Instance.AddPlayer(new BannerlordWrapper.Player(peer.Id.ToString(), peer.UserName, TeamType.Spectator));
            PlayerCosmetics playerCosmetics = EquipmentOverride.Instance.GetLoadedCosmeticsIfTheyExist(peer.Id.ToString());
            if (playerCosmetics != null)
            {
                PlayerWrapper.Instance.SetPlayerCosmetics(peer.Id.ToString(), playerCosmetics);
            }
        }

        protected override void OnPlayerDisconnect(VirtualPlayer peer)
        {
            PlayerWrapper.Instance.RemovePlayer(peer.Id.ToString());
        }

        public static void MissionStartUpdateWrappers()
        {
            string faction1 = MultiplayerOptions.OptionType.CultureTeam1.GetStrValue();
            TeamWrapper.Instance.SetFactionForTeam(TeamType.Attacker, faction1, GetIndexToTroopDictionary(MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(faction1)).ToList()));
            
            string faction2 = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue();
            TeamWrapper.Instance.SetFactionForTeam(TeamType.Defender, faction2, GetIndexToTroopDictionary(MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(faction2)).ToList()));
            
            PlayerWrapper.Instance.SetAllPlayersToSpectator();
        }

        private static Dictionary<int, Troop> GetIndexToTroopDictionary(List<MultiplayerClassDivisions.MPHeroClass> classes)
        {
            Dictionary<int, Troop> toReturn = new Dictionary<int, Troop>();
            int index = 0;
            foreach (var troopClass in classes)
            {
                Troop newTroop = new Troop(index, troopClass.TroopName.ToString(), GetTroopType(troopClass.ClassGroup.Name.ToString()));
                toReturn.Add(index, newTroop);
                index++;
            }

            return toReturn;
        }

        private static TroopType GetTroopType(string typeString)
        {
            Dictionary<string, TroopType> stringToTroopType = new Dictionary<string, TroopType>();
            stringToTroopType.Add(new TextObject("{=1Bm1Wk1v}Infantry").ToString(), TroopType.Infantry);
            stringToTroopType.Add(new TextObject("{=rangedtroop}Ranged").ToString(), TroopType.Ranged);
            stringToTroopType.Add(new TextObject("{=YVGtcLHF}Cavalry").ToString(), TroopType.Cavalry);
            stringToTroopType.Add(new TextObject("{=ugJfuabA}Horse Archer").ToString(), TroopType.HorseArcher);

            if (stringToTroopType.ContainsKey(typeString))
            {
                return stringToTroopType[typeString];
            }
            else
            {
                return TroopType.NotFound;
            }
        }

        public override void OnAfterSave()
        {

        }

        public override void OnBeforeSave()
        {

        }
    }
}
