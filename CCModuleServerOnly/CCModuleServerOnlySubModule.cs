using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ModuleManager;
using CCModuleNetworkMessages.FromServer;
using BannerlordWrapper;
using CCModuleServerOnly.Wrappers;
using TaleWorlds.ObjectSystem;
using TaleWorlds.Localization;
using TroopType = BannerlordWrapper.TroopType;

namespace CCModuleServerOnly
{
    public class CCModuleServerOnlySubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Setup();
        }

        private void Setup()
        {
            Debug.Print("Player Manager Loaded", 0, Debug.DebugColor.Magenta);
            PlayerManager.Instance.Setup();
            Logging.Instance.StartLogging("CCLogs", Logging.LogLevel.Debug);
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.InitializeGameStarter(game, starterObject);
            game.AddGameHandler<CCModuleGameHandler>();
            game.AddGameHandler<BannerlordWrapperGameHandler>();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
            game.AddGameHandler<ClientMessageHandler>();
        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            mission.AddMissionBehavior(new EquipmentOverrideMissionBehavior());
            TroopCapServerLogic.Instance.Setup();
            MissionStartUpdateWrappers();
        }

        private void MissionStartUpdateWrappers()
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

            if(stringToTroopType.ContainsKey(typeString))
            {
                return stringToTroopType[typeString];
            }
            else
            {
                return TroopType.NotFound;
            }
        }

    }
}
