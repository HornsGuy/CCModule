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
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer;
using TaleWorlds.MountAndBlade.View.Screens;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;
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
            game.AddGameHandler<ServerMessageHandler>();
        }

        

        public override void OnMultiplayerGameStart(Game game, object starterObject)
        {
            base.OnMultiplayerGameStart(game, starterObject);
        }

        bool temp = true;

        //protected override void OnApplicationTick(float dt)
        //{
        //    base.OnApplicationTick(dt);
        //    if(classSelectionView != null && temp == true)
        //    {

        //        FieldInfo type = typeof(MissionGauntletClassLoadout).GetField("_dataSource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        //        MultiplayerClassLoadoutVM vm = (MultiplayerClassLoadoutVM)type.GetValue(classSelectionView);
        //        if (vm != null)
        //        {
        //            foreach (var las in vm.Classes)
        //            {
        //                if(las.IsValid)
        //                {
        //                    foreach (var sc in las.SubClasses)
        //                    {
        //                        if(!sc.IsSelected)
        //                        {
        //                            sc.IsEnabled = false;
        //                        }
        //                    }
        //                }
        //            }

        //            ChatMessageManager.ServerMessage("Found loadout, yo");
        //            temp = false;
        //        }
        //    }
        //}

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            mission.AddMissionBehavior(new AdminPanelMissionView());
            mission.AddMissionBehavior(new TroopCapBehavior(mission.GetMissionBehavior<MissionGauntletClassLoadout>()));

        }

    }
}
