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
using HarmonyLib;
using System.Reflection;
using CCModuleServerOnly.HarmonyPatches;

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

            // Load maps
            MapLoader.LoadMaps("CCFiles\\gameTypes.txt", "CCFiles\\maps.csv");
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

        private void MissionHarmonyPatches()
        {

            var harmony = new Harmony("CCModule.SpawnEquipmentOverride");
            // harmony.PatchAll(assembly);
            var spawnAgentFunction = typeof(Mission).GetMethod("SpawnAgent", BindingFlags.Public | BindingFlags.Instance);
            var equipmentOverrideFunction = typeof(PatchMission).GetMethod("Prefix");
            harmony.Patch(spawnAgentFunction, prefix: new HarmonyMethod(equipmentOverrideFunction));

            var updateTroopIndex = typeof(MissionLobbyEquipmentNetworkComponent).GetMethod("HandleClientEventLobbyEquipmentUpdated", BindingFlags.NonPublic | BindingFlags.Instance);
            var checkTroopCaps = typeof(PatchMissionLobbyEquipmentNetworkComponent).GetMethod("Prefix");
            harmony.Patch(updateTroopIndex, prefix: new HarmonyMethod(checkTroopCaps));

            var changeGoldForPeer = typeof(MissionMultiplayerGameModeBase).GetMethod("ChangeCurrentGoldForPeer", BindingFlags.Public | BindingFlags.Instance);
            var overrideGold = typeof(PatchMissionMultiplayerGameModeBase).GetMethod("Prefix");
            harmony.Patch(changeGoldForPeer, prefix: new HarmonyMethod(overrideGold));

        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);
            EquipmentOverride.Instance.Setup();
            BannerlordWrapperGameHandler.MissionStartUpdateWrappers();
            MissionHarmonyPatches();
        }

    }
}
