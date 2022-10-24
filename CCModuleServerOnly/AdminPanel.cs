using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using TaleWorlds.MountAndBlade.DedicatedCustomServer;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.Source.Missions;
using NetworkMessages.FromServer;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;
using TaleWorlds.MountAndBlade.Diamond;
using System.Reflection;

namespace CCModuleServerOnly
{

    public class AdminPanelData
    {
        static AdminPanelData _instance;
        public static AdminPanelData Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new AdminPanelData();
                }
                return _instance;
            }
        }

        AdminPanelData()
        {
            InfantryCap = 100;
            RangedCap = 100;
            CavalryCap = 100;
        }

        public bool UpdateTroopCapsIfDifferent(int infCap, int rangeCap, int cavCap)
        {
            if (InfantryCap != infCap || RangedCap != rangeCap || CavalryCap != cavCap)
            {
                InfantryCap = infCap;
                RangedCap = rangeCap;
                CavalryCap = cavCap;
                return true;
            }
            return false;
        }

        public int InfantryCap { get; set; }
        public int RangedCap { get; set; }
        public int CavalryCap { get; set; }

    }

    public struct MissionData
    {
        public string gameType;
        public string mapId;
        public string cultureTeam1;
        public string cultureTeam2;
        public bool cultureVote;
        public bool mapVote;
        public int roundTime;
        public int warmupTime;
        public int mapTime;
        public int numRounds;

        public override string ToString()
        {
            return "gameType: " + gameType + "\n" +
                "mapId: " + mapId + "\n" +
                "cultureTeam1: " + cultureTeam1 + "\n" +
                "cultureTeam2: " + cultureTeam2 + "\n" +
                "cultureTeam2: " + cultureTeam2 + "\n" +
                "cultureVote: " + cultureVote + "\n" +
                "mapVote: " + mapVote + "\n" +
                "roundTime: " + roundTime + "\n" +
                "warmupTime: " + warmupTime + "\n" +
                "mapTime: " + mapTime + "\n" +
                "numRounds: " + numRounds + "\n";
        }
    }

    class StartMissionThread
    {
        // To successfully change the map, this thread must be called when the mission has ended
        public static void ThreadProc(Object missionData)
        {
            // Give us some buffer between the OnMissionEnd event and starting the next mission
            Thread.Sleep(500);

            // Prevent infinite loop if for some reason a call to StartMission 
            AdminPanel.Instance.StartMissionOnly((MissionData)missionData);
            AdminPanel.Instance.EndingCurrentMissionThenStartingNewMission = false;
        }
    }

    class MissionListener : IMissionListener
    {
        MissionData missionData;

        public void setMissionData(MissionData missionData)
        {
            this.missionData = missionData;
        }

        public void OnConversationCharacterChanged()
        {

        }

        public void OnEndMission()
        {
            // Run a thread that will create a start a mission after a delay
            Thread t = new Thread(new ParameterizedThreadStart(StartMissionThread.ThreadProc));
            t.Start(missionData);

            Mission.Current.RemoveListener(this);
        }

        public void OnEquipItemsFromSpawnEquipment(Agent agent, Agent.CreationType creationType)
        {

        }

        public void OnEquipItemsFromSpawnEquipmentBegin(Agent agent, Agent.CreationType creationType)
        {

        }

        public void OnInitialDeploymentPlanMade(BattleSideEnum battleSide, bool isFirstPlan)
        {

        }

        public void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
        {

        }

        public void OnResetMission()
        {

        }
    }

    // TODO: A lot of the find/search functions are all the same. See if we can refactor that down a bit
    public class AdminPanel
    {

        // Singleton
        private static AdminPanel instance;
        public static AdminPanel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AdminPanel();
                }
                return instance;
            }
        }

        public bool MissionIsRunning
        {
            get
            {
                return Mission.Current != null;
            }
        }

        // Prevent multiple missions from being started at once
        public bool EndingCurrentMissionThenStartingNewMission = false;

        string GetOptionString(MultiplayerOptions.OptionType optionType)
        {
            string toReturn;
            MultiplayerOptions.Instance.GetOptionFromOptionType(optionType).GetValue(out toReturn);
            return toReturn;
        }

        int GetOptionInt(MultiplayerOptions.OptionType optionType)
        {
            int toReturn;
            MultiplayerOptions.Instance.GetOptionFromOptionType(optionType).GetValue(out toReturn);
            return toReturn;
        }

        MissionData getMultiplayerOptionsState()
        {
            MissionData toReturn = new MissionData();

            toReturn.cultureVote = MultiplayerIntermissionVotingManager.Instance.IsCultureVoteEnabled;
            toReturn.mapVote = MultiplayerIntermissionVotingManager.Instance.IsMapVoteEnabled;
            toReturn.cultureTeam1 = GetOptionString(MultiplayerOptions.OptionType.CultureTeam1);
            toReturn.cultureTeam2 = GetOptionString(MultiplayerOptions.OptionType.CultureTeam2);
            toReturn.mapId = GetOptionString(MultiplayerOptions.OptionType.Map);
            toReturn.gameType = GetOptionString(MultiplayerOptions.OptionType.GameType);
            toReturn.roundTime = GetOptionInt(MultiplayerOptions.OptionType.RoundTimeLimit);
            toReturn.warmupTime = GetOptionInt(MultiplayerOptions.OptionType.WarmupTimeLimit);
            toReturn.mapTime = GetOptionInt(MultiplayerOptions.OptionType.MapTimeLimit);
            toReturn.numRounds = GetOptionInt(MultiplayerOptions.OptionType.RoundTotal);

            return toReturn;
        }

        List<string> GetMapsForCurrentGameType()
        {
            return MultiplayerOptions.Instance.GetMultiplayerOptionsList(MultiplayerOptions.OptionType.Map);
        }

        List<string> GetMapsInPool()
        {
            return MultiplayerIntermissionVotingManager.Instance.MapVoteItems.Select(kvp => kvp.Key).ToList();
        }

        public List<string> GetAllAvailableMaps()
        {
            return GetMapsForCurrentGameType().Union(GetMapsInPool()).ToList();
        }

        List<string> FindMaps(string searchString)
        {
            return GetAllAvailableMaps().Where(str => str.Contains(searchString)).ToList();
        }

        public Tuple<bool, string> FindSingleMap(string searchString)
        {
            List<string> foundMaps = FindMaps(searchString);

            if (foundMaps.Count == 1)
            {
                return new Tuple<bool, string>(true, foundMaps[0]);
            }
            else if (foundMaps.Count > 1)
            {
                // Check for special case where the name of a map sits inside the name of another map ie mp_tdm_map_001 or mp_tdm_map_001_spring
                foreach (string mapName in foundMaps)
                {
                    if (mapName == searchString)
                    {
                        return new Tuple<bool, string>(true, mapName);
                    }
                }

                return new Tuple<bool, string>(false, "More than one map found matching '" + searchString + "'");
            }
            else
            {
                return new Tuple<bool, string>(false, "No maps found matching '" + searchString + "'");
            }
        }

        public void ChangeMap(string mapId)
        {
            MissionData currentState = getMultiplayerOptionsState();
            currentState.mapId = mapId;
            StartMission(currentState);
        }

        public List<string> GetAllFactions()
        {
            return MultiplayerOptions.Instance.GetMultiplayerOptionsList(MultiplayerOptions.OptionType.CultureTeam1);
        }

        public List<string> FindMatchingFactions(string searchString)
        {
            List<string> availableFactions = GetAllFactions();

            return availableFactions.Where(str => str.Contains(searchString)).ToList();
        }

        public Tuple<bool, string> FindSingleFaction(string searchString)
        {
            List<string> foundFactions = FindMatchingFactions(searchString);

            if (foundFactions.Count == 1)
            {
                return new Tuple<bool, string>(true, foundFactions[0]);
            }
            else if (foundFactions.Count > 1)
            {
                return new Tuple<bool, string>(false, "More than one faction found matching '" + searchString + "'");
            }
            else
            {
                return new Tuple<bool, string>(false, "No factions found matching '" + searchString + "'");
            }
        }

        public List<string> GetGameTypes()
        {
            return MultiplayerOptions.Instance.GetMultiplayerOptionsList(MultiplayerOptions.OptionType.GameType);
        }

        private List<string> GetMatchingGameTypes(string searchString)
        {
            List<string> availableGameTypes = GetGameTypes();

            return availableGameTypes.Where(str => str.Contains(searchString)).ToList();
        }

        public Tuple<bool, string> FindSingleGameType(string searchString)
        {
            List<string> foundGameTypes = GetMatchingGameTypes(searchString);

            if (foundGameTypes.Count == 1)
            {
                return new Tuple<bool, string>(true, foundGameTypes[0]);
            }
            else if (foundGameTypes.Count > 1)
            {
                return new Tuple<bool, string>(false, "More than one game type found matching '" + searchString + "'");
            }
            else
            {
                return new Tuple<bool, string>(false, "No game types found matching '" + searchString + "'");
            }
        }

        public List<string> GetMapsForGameType(string searchString)
        {

            Tuple<bool, string> gameTypeSearch = FindSingleGameType(searchString);
            if (gameTypeSearch.Item1)
            {
                return MultiplayerGameTypes.GetGameTypeInfo(gameTypeSearch.Item2).Scenes.ToList().Union(GetMapsInPool()).ToList();
            }
            return new List<string>();
        }


        public Tuple<bool, string> FindMapForGameType(string gameType, string searchString)
        {
            List<string> foundMaps = GetMapsForGameType(gameType);

            List<string> filtered = foundMaps.Where(str => str.Contains(searchString)).ToList(); ;

            if (filtered.Count == 1)
            {
                return new Tuple<bool, string>(true, filtered[0]);
            }
            else if (filtered.Count > 1)
            {
                // Check for special case where the name of a map sits inside the name of another map ie mp_tdm_map_001 or mp_tdm_map_001_spring
                foreach (string mapName in filtered)
                {
                    if (mapName == searchString)
                    {
                        return new Tuple<bool, string>(true, mapName);
                    }
                }

                return new Tuple<bool, string>(false, "More than one map found matching '" + searchString + "'");
            }
            else
            {
                return new Tuple<bool, string>(false, "No maps found matching '" + searchString + "'");
            }
        }

        // NOTE: Does not verify if the current map matches the game type!
        public void ChangeGameTypeMapFactions(string gameType, string mapId, string faction1, string faction2)
        {
            MissionData currentState = getMultiplayerOptionsState();
            currentState.gameType = gameType;
            currentState.mapId = mapId;
            currentState.cultureTeam1 = faction1;
            currentState.cultureTeam2 = faction2;
            StartMission(currentState);
        }

        public void ChangeMapAndFactions(string mapId, string faction1, string faction2)
        {
            MissionData currentState = getMultiplayerOptionsState();
            currentState.mapId = mapId;
            currentState.cultureTeam1 = faction1;
            currentState.cultureTeam2 = faction2;
            StartMission(currentState);
        }

        public void BroadcastMessage(string message)
        {
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage(new ServerMessage(message));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
        }

        public void ResetMission()
        {
            Mission.Current.ResetMission();
        }

        public void SetRoundTime(int roundTime)
        {
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.RoundTimeLimit).UpdateValue(roundTime);

            MissionLobbyComponent mlc = Mission.Current.GetMissionBehavior<MissionLobbyComponent>();

            if (mlc != null)
            {
                MultiplayerTimerComponent timer = GetMissionTimer();

                timer.StartTimerAsServer((float)(roundTime));

                SyncMultiplayerOptions();

                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage((GameNetworkMessage)new MissionStateChange(mlc.CurrentMultiplayerState, timer.GetCurrentTimerStartTime().NumberOfTicks));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
            }
        }

        private MultiplayerWarmupComponent.WarmupStates? GetWarmupState()
        {
            MultiplayerWarmupComponent mwc = Mission.Current.GetMissionBehavior<MultiplayerWarmupComponent>();

            if (mwc != null)
            {
                FieldInfo type = typeof(MultiplayerWarmupComponent).GetField("_warmupState", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (MultiplayerWarmupComponent.WarmupStates)type.GetValue(mwc);
            }

            return null;
        }

        private MultiplayerTimerComponent GetWarmupTimer()
        {
            MultiplayerWarmupComponent mwc = Mission.Current.GetMissionBehavior<MultiplayerWarmupComponent>();

            if (mwc != null)
            {
                FieldInfo type = typeof(MultiplayerWarmupComponent).GetField("_timerComponent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (MultiplayerTimerComponent)type.GetValue(mwc);
            }

            return null;
        }

        public void SetWarmupTime(int warmupTime)
        {
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.WarmupTimeLimit).UpdateValue(warmupTime);

            MultiplayerWarmupComponent mwc = Mission.Current.GetMissionBehavior<MultiplayerWarmupComponent>();

            if (mwc != null)
            {

                if (GetWarmupState() == MultiplayerWarmupComponent.WarmupStates.InProgress)
                {

                    MultiplayerTimerComponent timer = GetWarmupTimer();
                    timer.StartTimerAsServer((float)(warmupTime * 60));

                    SyncMultiplayerOptions();

                    GameNetwork.BeginBroadcastModuleEvent();
                    GameNetwork.WriteMessage((GameNetworkMessage)new WarmupStateChange(MultiplayerWarmupComponent.WarmupStates.InProgress, timer.GetCurrentTimerStartTime().NumberOfTicks));
                    GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
                    Debug.Print("Changed!!!!", 0, Debug.DebugColor.Magenta);
                }
            }
        }

        private void SyncMultiplayerOptions()
        {
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage)new MultiplayerOptionsInitial());
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);

            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage)new MultiplayerOptionsImmediate());
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
        }

        private MultiplayerTimerComponent GetMissionTimer()
        {
            MissionLobbyComponent mlc = Mission.Current.GetMissionBehavior<MissionLobbyComponent>();

            if (mlc != null)
            {
                Type typ = typeof(MissionLobbyComponent);
                FieldInfo type = typ.GetField("_timerComponent", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                return (MultiplayerTimerComponent)type.GetValue(mlc);
            }

            return null;
        }

        public void SetMapTime(int mapTime)
        {
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.MapTimeLimit).UpdateValue(mapTime);

            MissionLobbyComponent mlc = Mission.Current.GetMissionBehavior<MissionLobbyComponent>();

            if (mlc != null)
            {
                MultiplayerTimerComponent timer = GetMissionTimer();

                timer.StartTimerAsServer((float)(mapTime * 60));

                SyncMultiplayerOptions();

                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage((GameNetworkMessage)new MissionStateChange(mlc.CurrentMultiplayerState, timer.GetCurrentTimerStartTime().NumberOfTicks));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
            }

        }

        public void SetMultiplayerOptions(MissionData missionData, MultiplayerOptions.MultiplayerOptionsAccessMode opetionSet = MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions)
        {
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType, opetionSet).UpdateValue(missionData.gameType);
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.Map, opetionSet).UpdateValue(missionData.mapId);
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam1, opetionSet).UpdateValue(missionData.cultureTeam1);
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam2, opetionSet).UpdateValue(missionData.cultureTeam2);
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.RoundTimeLimit, opetionSet).UpdateValue(missionData.roundTime);
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.WarmupTimeLimit, opetionSet).UpdateValue(missionData.warmupTime);
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.MapTimeLimit, opetionSet).UpdateValue(missionData.mapTime);
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.RoundTotal, opetionSet).UpdateValue(missionData.numRounds);
            MultiplayerIntermissionVotingManager.Instance.IsCultureVoteEnabled = missionData.cultureVote;
            MultiplayerIntermissionVotingManager.Instance.IsMapVoteEnabled = missionData.mapVote;
            MultiplayerIntermissionVotingManager.Instance.ClearVotes();
            MultiplayerIntermissionVotingManager.Instance.SetVotesOfCulture(missionData.cultureTeam1, 100);
            MultiplayerIntermissionVotingManager.Instance.SetVotesOfCulture(missionData.cultureTeam2, 100);
            Debug.Print(missionData.ToString(), 0, Debug.DebugColor.Yellow);
        }

        private void SyncMultiplayerOptionsToClients()
        {
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage)new MultiplayerOptionsInitial());
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.IncludeUnsynchronizedClients, (NetworkCommunicator)null);
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage((GameNetworkMessage)new MultiplayerOptionsImmediate());
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.IncludeUnsynchronizedClients, (NetworkCommunicator)null);
        }

        private void ResetFactionVoteCount()
        {
            MultiplayerIntermissionVotingManager.Instance.ClearVotes();
        }

        public void StartMission(MissionData missionData)
        {
            if (!EndingCurrentMissionThenStartingNewMission)
            {
                if (!MissionIsRunning)
                {
                    StartMissionOnly(missionData);
                }
                else
                {
                    EndMissionThenStartMission(missionData);
                }
            }
        }

        public void EndWarmup()
        {
            MultiplayerWarmupComponent warmup = Mission.Current.GetMissionBehavior<MultiplayerWarmupComponent>();

            if (warmup != null)
            {
                typeof(MultiplayerWarmupComponent).GetMethod("EndWarmup", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(warmup, null);
            }
        }

        private void EndMissionThenStartMission(MissionData missionData)
        {
            MissionListener listener = new MissionListener();
            Mission.Current.AddListener(listener);

            MultiplayerIntermissionVotingManager.Instance.IsCultureVoteEnabled = false;
            MultiplayerIntermissionVotingManager.Instance.IsMapVoteEnabled = false;

            EndingCurrentMissionThenStartingNewMission = true;

            EndWarmup();

            listener.setMissionData(missionData);
            DedicatedCustomServerSubModule.Instance.EndMission();
        }

        public bool StartMissionOnly(MissionData missionData)
        {
            if (!MissionIsRunning)
            {
                SetMultiplayerOptions(missionData);
                DedicatedCustomServerSubModule.Instance.StartMission();
                SyncMultiplayerOptionsToClients();
                ResetFactionVoteCount();
                return true;
            }

            return false;
        }

        public bool EndMission()
        {
            if (MissionIsRunning)
            {
                DedicatedCustomServerSubModule.Instance.StartMission();
            }

            return false;
        }

        public void SetBots(int team1, int team2)
        {
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.NumberOfBotsTeam1).UpdateValue(team1);
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.NumberOfBotsTeam2).UpdateValue(team2);
        }

        protected BodyProperties GetBodyProperties(
      MissionPeer missionPeer,
      BasicCultureObject cultureLimit)
        {
            NetworkCommunicator networkPeer = missionPeer.GetNetworkPeer();
            if (networkPeer != null)
                return networkPeer.PlayerConnectionInfo.GetParameter<PlayerData>("PlayerData").BodyProperties;
            Debug.FailedAssert("networkCommunicator != null", "C:\\Develop\\MB3\\Source\\Bannerlord\\TaleWorlds.MountAndBlade\\Missions\\Multiplayer\\SpawnBehaviors\\SpawningBehaviors\\SpawningBehaviorBase.cs", nameof(GetBodyProperties), 366);
            Team team = missionPeer.Team;
            BasicCharacterObject troopCharacter = MultiplayerClassDivisions.GetMPHeroClasses(cultureLimit).ToList<MultiplayerClassDivisions.MPHeroClass>().GetRandomElement<MultiplayerClassDivisions.MPHeroClass>().TroopCharacter;
            MatrixFrame spawnFrame = Mission.Current.GetMissionBehavior<SpawnComponent>().GetSpawnFrame(team, troopCharacter.HasMount(), true);
            AgentBuildData agentBuildData1 = new AgentBuildData(troopCharacter).Team(team).InitialPosition(in spawnFrame.origin);
            Vec2 vec2 = spawnFrame.rotation.f.AsVec2;
            vec2 = vec2.Normalized();
            ref Vec2 local = ref vec2;
            AgentBuildData agentBuildData2 = agentBuildData1.InitialDirection(in local).TroopOrigin((IAgentOriginBase)new BasicBattleAgentOrigin(troopCharacter)).EquipmentSeed(Mission.Current.GetMissionBehavior<MissionLobbyComponent>().GetRandomFaceSeedForCharacter(troopCharacter, 0)).ClothingColor1(team.Side == BattleSideEnum.Attacker ? cultureLimit.Color : cultureLimit.ClothAlternativeColor).ClothingColor2(team.Side == BattleSideEnum.Attacker ? cultureLimit.Color2 : cultureLimit.ClothAlternativeColor2).IsFemale(troopCharacter.IsFemale);
            agentBuildData2.Equipment(Equipment.GetRandomEquipmentElements(troopCharacter, !(Game.Current.GameType is MultiplayerGame), false, agentBuildData2.AgentEquipmentSeed));
            agentBuildData2.BodyProperties(BodyProperties.GetRandomBodyProperties(agentBuildData2.AgentRace, agentBuildData2.AgentIsFemale, troopCharacter.GetBodyPropertiesMin(false), troopCharacter.GetBodyPropertiesMax(), (int)agentBuildData2.AgentOverridenSpawnEquipment.HairCoverType, agentBuildData2.AgentEquipmentSeed, troopCharacter.HairTags, troopCharacter.BeardTags, troopCharacter.TattooTags));
            return agentBuildData2.AgentBodyProperties;
        }

        protected Tuple<AgentBuildData, int> GetAgentBuildDataForPlayer(NetworkCommunicator networkPeer)
        {
            BasicCultureObject cultureLimit1 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));
            BasicCultureObject cultureLimit2 = MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue(MultiplayerOptions.MultiplayerOptionsAccessMode.CurrentMapOptions));

            MissionPeer component = networkPeer.GetComponent<MissionPeer>();

            IAgentVisual agentVisualForPeer = component.GetAgentVisualForPeer(0);
            BasicCultureObject basicCultureObject = component.Team.Side == BattleSideEnum.Attacker ? cultureLimit1 : cultureLimit2;
            int num = component.SelectedTroopIndex;
            IEnumerable<MultiplayerClassDivisions.MPHeroClass> mpHeroClasses = MultiplayerClassDivisions.GetMPHeroClasses(basicCultureObject);
            MultiplayerClassDivisions.MPHeroClass mpHeroClass = num < 0 ? (MultiplayerClassDivisions.MPHeroClass)null : mpHeroClasses.ElementAt<MultiplayerClassDivisions.MPHeroClass>(num);
            if (mpHeroClass == null && num < 0)
            {
                mpHeroClass = mpHeroClasses.First<MultiplayerClassDivisions.MPHeroClass>();
                num = 0;
            }
            BasicCharacterObject heroCharacter = mpHeroClass.HeroCharacter;
            Equipment equipment = heroCharacter.Equipment.Clone(false);
            IEnumerable<ValueTuple<EquipmentIndex, EquipmentElement>> alternativeEquipments = MPPerkObject.GetOnSpawnPerkHandler(component)?.GetAlternativeEquipments(true);
            if (alternativeEquipments != null)
            {
                foreach (ValueTuple<EquipmentIndex, EquipmentElement> valueTuple in alternativeEquipments)
                    equipment[valueTuple.Item1] = valueTuple.Item2;
            }
            MatrixFrame matrixFrame;
            if (agentVisualForPeer == null)
            {

                matrixFrame = Mission.Current.GetMissionBehavior<SpawnComponent>().GetSpawnFrame(component.Team, heroCharacter.Equipment.Horse.Item != null, false);
            }
            else
            {
                matrixFrame = agentVisualForPeer.GetFrame();
                matrixFrame.rotation.MakeUnit();
            }

            AgentBuildData agentBuildData1 = new AgentBuildData(heroCharacter).MissionPeer(component).Equipment(equipment).Team(component.Team).TroopOrigin((IAgentOriginBase)new BasicBattleAgentOrigin(heroCharacter)).InitialPosition(in matrixFrame.origin);
            Vec2 vec2 = matrixFrame.rotation.f.AsVec2.Normalized();
            ref Vec2 local = ref vec2;
            return new Tuple<AgentBuildData, int>(agentBuildData1.InitialDirection(in local).IsFemale(component.Peer.IsFemale).BodyProperties(this.GetBodyProperties(component, basicCultureObject)).VisualsIndex(0).ClothingColor1(component.Team == Mission.Current.AttackerTeam ? basicCultureObject.Color : basicCultureObject.ClothAlternativeColor).ClothingColor2(component.Team == Mission.Current.AttackerTeam ? basicCultureObject.Color2 : basicCultureObject.ClothAlternativeColor2), num);

        }

        public NetworkCommunicator GetPlayerNetworkPeerFromID(string ID)
        {
            foreach (var peer in GameNetwork.NetworkPeers)
            {
                if (peer.VirtualPlayer.Id.ToString() == ID)
                {
                    return peer;
                }
            }

            return null;
        }

        public bool GivePlayerAgentCosmeticEquipment(string playerID, List<Tuple<EquipmentIndex, string>> itemsToGive)
        {
            NetworkCommunicator peer = GetPlayerNetworkPeerFromID(playerID);
            if (peer != null)
            {
                return GivePlayerAgentCosmeticEquipment(peer, itemsToGive);
            }
            return false;
        }

        public void KickPlayer(NetworkCommunicator peer)
        {
            DedicatedCustomServerSubModule.Instance.DedicatedCustomGameServer.KickPlayer(peer.VirtualPlayer.Id, false);
        }

        public bool GivePlayerAgentCosmeticEquipment(NetworkCommunicator networkPeer, List<Tuple<EquipmentIndex, string>> itemsToGive)
        {
            if (networkPeer.ControlledAgent != null)
            {
                Agent oldAgent = networkPeer.ControlledAgent;
                bool wasRidingHorse = !oldAgent.SpawnEquipment[EquipmentIndex.Horse].IsEmpty;

                Vec3 OriginalPos = oldAgent.Position;
                Tuple<AgentBuildData, int> retVal = GetAgentBuildDataForPlayer(networkPeer);

                AgentBuildData bda = retVal.Item1;

                // Set position and look direction
                bda = bda.InitialPosition(OriginalPos);
                Vec3 lookDir3 = oldAgent.LookDirection;
                Vec2 lookDir = lookDir3.AsVec2;
                bda = bda.InitialDirection(lookDir);

                Equipment newEquipment = new Equipment(networkPeer.ControlledAgent.Character.Equipment.Clone());

                foreach (var itemToGive in itemsToGive)
                {
                    ItemObject item = MBObjectManager.Instance.GetObject<ItemObject>(itemToGive.Item2);
                    if (item != null)
                    {
                        EquipmentElement newItemElement = newEquipment[itemToGive.Item1];
                        newItemElement.CosmeticItem = item;
                        newEquipment[itemToGive.Item1] = newItemElement;
                    }
                    // If we pass an empty string, clear the armor
                    if (itemToGive.Item2 == "")
                    {
                        newEquipment[itemToGive.Item1] = new EquipmentElement();
                    }
                }

                // Get selected for everything else
                newEquipment[EquipmentIndex.Horse] = oldAgent.SpawnEquipment[EquipmentIndex.Horse];
                newEquipment[EquipmentIndex.HorseHarness] = oldAgent.SpawnEquipment[EquipmentIndex.HorseHarness];

                newEquipment[EquipmentIndex.Weapon0] = oldAgent.SpawnEquipment[EquipmentIndex.Weapon0];
                newEquipment[EquipmentIndex.Weapon1] = oldAgent.SpawnEquipment[EquipmentIndex.Weapon1];
                newEquipment[EquipmentIndex.Weapon2] = oldAgent.SpawnEquipment[EquipmentIndex.Weapon2];
                newEquipment[EquipmentIndex.Weapon3] = oldAgent.SpawnEquipment[EquipmentIndex.Weapon3];
                newEquipment[EquipmentIndex.Weapon4] = oldAgent.SpawnEquipment[EquipmentIndex.Weapon4];


                // Override the equipment now that cosmetics are placed
                bda = bda.Equipment(newEquipment);



                // Spawning the agent, player immediately takes control
                Agent newAgent = Mission.Current.SpawnAgent(bda);

                // Make sure we wield the default items
                newAgent.WieldInitialWeapons();

                // Before we can remove the old agent, we need to increment the number of bots alive on the scoreboard.
                // Oversight on Taleworlds' part
                Mission.Current.GetMissionBehavior<MissionScoreboardComponent>().Sides[(int)(networkPeer.GetComponent<MissionPeer>().Team.Side)].BotScores.AliveCount += 1;

                // Fade old agent and horse if ncessary
                oldAgent.FadeOut(true, wasRidingHorse);

                return true;
            }
            return false;
        }
    }
}
