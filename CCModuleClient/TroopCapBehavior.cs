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
    public class TroopCapBehavior : MissionBehavior
    {
        MissionGauntletClassLoadout _loadoutMissionView;
        MultiplayerClassLoadoutVM _vm;

        public delegate void OnClassLoadoutUIOpenedEvent();

        public event OnClassLoadoutUIOpenedEvent OnClassLoadoutUIOpened;

        public bool uiOpened = false;

        private Dictionary<string, int> troopTypePercent = new Dictionary<string, int>();

        public static void UpdateTroopCaps(int infCap, int rangeCap, int cavCap)
        {
            if(Mission.Current != null)
            {
                TroopCapBehavior troopCapBehavior = Mission.Current.GetMissionBehavior<TroopCapBehavior>();
                if(troopCapBehavior != null)
                {
                    troopCapBehavior.UpdateTroopCapsInternal(infCap, rangeCap, cavCap);
                    troopCapBehavior.RefreshLoadoutVM();
                }
            }
        }

        public TroopCapBehavior(MissionGauntletClassLoadout loadoutMissionView, int infCap, int rangeCap, int cavCap)
        {
            _loadoutMissionView = loadoutMissionView;
            OnClassLoadoutUIOpened += RefreshLoadoutVM;

            // Defaults
            troopTypePercent.Add("Infantry", infCap);
            troopTypePercent.Add("Ranged", rangeCap);
            troopTypePercent.Add("Cavalry", cavCap);
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

        public static Dictionary<string,float> GetCurrentTeamClassTypeBreakdown(List<int> myTeamTroopIndexes, Dictionary<int, string> troopIndexToTroopType)
        {

            // Add the default 3 we care about
            Dictionary<string, float> troopTypeCount = new Dictionary<string, float>();
            troopTypeCount.Add("Infantry",0);
            troopTypeCount.Add("Ranged",0);
            troopTypeCount.Add("Cavalry",0);

            float total = 0;
            foreach (var troopIndex in myTeamTroopIndexes)
            {
                troopTypeCount[troopIndexToTroopType[troopIndex]] += 1;
                total += 1;
            }

            // Take the totals and convert it into the breakdown
            Dictionary<string, float> toReturn = new Dictionary<string, float>();

            foreach (var keyVal in troopTypeCount)
            {
                toReturn.Add(keyVal.Key, (keyVal.Value / total) * 100.0f);
            }

            return toReturn;
        }

        public static Dictionary<string,bool> GetTroopClassAvailabilityDictionary(Dictionary<string, float> currentTroopBreakdown, Dictionary<string, int> troopCapPercent)
        {
            Dictionary<string, bool> toReturn = new Dictionary<string, bool>();

            foreach (var keyVal in currentTroopBreakdown)
            {
                toReturn.Add(keyVal.Key, keyVal.Value < troopCapPercent[keyVal.Key]);
            }

            return toReturn;
        }

        private Dictionary<int, string> GetTroopIndexToTroopTypeDictionary(List<HeroClassGroupVM> classGroups)
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>();

            int troopIndex = 0;
            foreach (var troopTypeGroup in classGroups)
            {
                foreach (var troopClass in troopTypeGroup.SubClasses)
                {
                    toReturn.Add(troopIndex, troopTypeGroup.Name);
                    troopIndex++;
                }
            }

            return toReturn;
        }

        private void UpdateTroopCapsInternal(int infCap, int rangeCap, int cavCap)
        {
            troopTypePercent["Infantry"] = infCap;
            troopTypePercent["Ranged"] = rangeCap;
            troopTypePercent["Cavalry"] = cavCap;
        }

        public void RefreshLoadoutVM()
        {
            if(_vm != null)
            {
                ResetVM();
                Dictionary<string, float> currentTroopBreakdown = GetCurrentTeamClassTypeBreakdown(PlayerWrapper.GetMyTeamTroopIndeces(),GetTroopIndexToTroopTypeDictionary(_vm.Classes.ToList()));
                Dictionary<string, bool> classIsAvailable = GetTroopClassAvailabilityDictionary(currentTroopBreakdown, troopTypePercent);
                foreach (var troopTypeGroup in _vm.Classes)
                {
                    int currentTypePercent = troopTypePercent[troopTypeGroup.Name];
                    if(currentTypePercent != 100)
                    {
                        bool shouldBeLocked = !classIsAvailable[troopTypeGroup.Name];
                        foreach (var troopClass in troopTypeGroup.SubClasses)
                        {
                            if(shouldBeLocked)
                            {
                                troopClass.IsEnabled = false;
                                if(troopClass.IsSelected)
                                {
                                    troopClass.IsSelected = false;
                                    MissionPeer mp = GameNetwork.MyPeer.GetComponent<MissionPeer>();
                                    mp.SelectedTroopIndex = 0; // TODO: This won't work if Infantry Cap is set to 0
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
