using CCModuleNetworkMessages.FromClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;
using TaleWorlds.ObjectSystem;

namespace CCModuleClient
{
    public class CCModuleClientSubModule : MBSubModuleBase
    {

        public static bool playerIsAdmin = false;

        public override void OnGameInitializationFinished(Game game)
        {
            base.OnGameInitializationFinished(game);
            
        }

        public override void OnInitialState()
        {
            base.OnInitialState();
        }

        protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
        {
            base.OnGameStart(game, gameStarterObject);
        }

        

        public override void OnMultiplayerGameStart(Game game, object starterObject)
        {
            base.OnMultiplayerGameStart(game, starterObject);
            

        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            mission.AddMissionBehavior(new AdminPanelMissionView());
            mission.AddMissionBehavior(new AdminPanelNetworkMessagesClient());

            ChatMessageManager.AddMessage("CCModule is running", 55, 189, 40);
        }

    }
}
