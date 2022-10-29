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
            TeamWrapper.Instance.SetFactionForTeam(TeamWrapper.TeamType.Attacker, faction1, GetIndexToTroopNameDictionary(MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(faction1)).ToList()));
            string faction2 = MultiplayerOptions.OptionType.CultureTeam2.GetStrValue();
            TeamWrapper.Instance.SetFactionForTeam(TeamWrapper.TeamType.Defender, faction2, GetIndexToTroopNameDictionary(MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(faction2)).ToList()));
            PlayerWrapper.Instance.SetAllPlayersToSpectator();
        }

        private static Dictionary<int, string> GetIndexToTroopNameDictionary(List<MultiplayerClassDivisions.MPHeroClass> classes)
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>();
            int index = 0;
            foreach (var troopClass in classes)
            {
                toReturn.Add(index, troopClass.TroopName.ToString());
                index++;
            }

            return toReturn;
        }

    }
}
