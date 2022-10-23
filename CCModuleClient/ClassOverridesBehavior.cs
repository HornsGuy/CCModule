using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;

namespace CCModuleClient
{
    class TroopCapBehavior : MissionBehavior
    {
        MissionGauntletClassLoadout _loadoutMissionView;
        MultiplayerClassLoadoutVM _vm;

        public delegate void OnClassLoadoutUIOpenedEvent();

        public event OnClassLoadoutUIOpenedEvent OnClassLoadoutUIOpened;

        public bool uiOpened = false;

        private Dictionary<string, int> troopTypePercent = new Dictionary<string, int>();

        public TroopCapBehavior(MissionGauntletClassLoadout loadoutMissionView)
        {
            _loadoutMissionView = loadoutMissionView;
            OnClassLoadoutUIOpened += RefreshVM;
        }

        public override void OnMissionTick(float dt)
        {
            FieldInfo type = typeof(MissionGauntletClassLoadout).GetField("_dataSource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            _vm = (MultiplayerClassLoadoutVM)type.GetValue(_loadoutMissionView);
            if(_vm != null && !uiOpened)
            {
                OnClassLoadoutUIOpened();
                uiOpened = true;
            }

            // Remove latch is UI closes
            if(_vm == null && uiOpened)
            {
                uiOpened = false;
            }
        }

        // Need to add network message when server tells clients that the troop cap has changed

        private void ResetVM()
        {
            if(_vm != null)
            {
                _vm.OnGoldUpdated();
            }   
        }

        private Dictionary<string,float> GetCurrentTeamClassTypeBreakdown()
        {
            Dictionary<int, string> troopTypeCategoriesForFaction = GetCurrentFactionTroopIndexToTroopTypeDictionary();

            MissionPeer myMissionPeer = GameNetwork.MyPeer.GetComponent<MissionPeer>();

            // Add the default 3 we care about
            Dictionary<string, float> troopTypeCount = new Dictionary<string, float>();
            troopTypeCount.Add("Infantry",0);
            troopTypeCount.Add("Range",0);
            troopTypeCount.Add("Cavalry",0);

            float total = 0;
            foreach (var peer in GameNetwork.NetworkPeers)
            {
                MissionPeer mp = peer.GetComponent<MissionPeer>();
                
                // Only check players on our team
                if(mp != null && mp.Team.Side == myMissionPeer.Team.Side)
                {
                    troopTypeCount[troopTypeCategoriesForFaction[mp.SelectedTroopIndex]] += 1;
                    total += 1;
                }
            }

            // Take the totals and convert it into the breakdown
            Dictionary<string, float> toReturn = new Dictionary<string, float>();

            foreach (var keyVal in troopTypeCount)
            {
                toReturn.Add(keyVal.Key, (keyVal.Value / total) * 100.0f);
            }

            return toReturn;
        }

        public Dictionary<int, string> GetCurrentFactionTroopIndexToTroopTypeDictionary()
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>();

            int troopIndex = 0;
            foreach (var troopTypeGroup in _vm.Classes)
            {
                foreach (var troopClass in troopTypeGroup.SubClasses)
                {
                    toReturn.Add(troopIndex, troopTypeGroup.Name);
                    troopIndex++;
                }
            }

            return toReturn;
        }

        public void RefreshVM()
        {
            if(_vm != null)
            {
                ResetVM();
                Dictionary<string, float> currentTroopBreakdown = GetCurrentTeamClassTypeBreakdown();
                foreach (var troopTypeGroup in _vm.Classes)
                {
                    int currentTypePercent = troopTypePercent[troopTypeGroup.Name];
                    if(currentTypePercent != 100)
                    {
                        bool shouldBeLocked = currentTroopBreakdown[troopTypeGroup.Name] > currentTypePercent;
                        foreach (var troopClass in troopTypeGroup.SubClasses)
                        {
                            if(shouldBeLocked)
                            {
                                troopClass.IsEnabled = false;
                                if(troopClass.IsSelected)
                                {
                                    troopClass.IsSelected = false;
                                    MissionPeer mp = GameNetwork.MyPeer.GetComponent<MissionPeer>();
                                    mp.SelectedTroopIndex = 0; // TODO: This won't work if Infantry is set to 0
                                }
                            }
                        }
                    }
                }
            }
        }

        public override MissionBehaviorType BehaviorType => MissionBehaviorType.Other;
    }
}
