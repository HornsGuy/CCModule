using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.ModuleManager;

namespace CCModuleServerOnly
{
    public class CCModuleServerOnlySubModule : MBSubModuleBase
    {
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();
            Debug.Print("Player Manager Loaded", 0, Debug.DebugColor.Magenta);
            PlayerManager.Instance.Setup();
        }

        protected override void InitializeGameStarter(Game game, IGameStarter starterObject)
        {
            base.InitializeGameStarter(game, starterObject);
            game.AddGameHandler<CCModuleGameHandler>();
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
        }

    }
}
