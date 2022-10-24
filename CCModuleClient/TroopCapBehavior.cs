using ClientServerShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.GauntletUI.Mission.Multiplayer;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer.ClassLoadout;
using TaleWorlds.ObjectSystem;

namespace CCModuleClient
{
    public class TroopCapBehavior : MissionBehavior
    {
        MissionGauntletClassLoadout _loadoutMissionView;
        MultiplayerClassLoadoutVM _vm;

        public delegate void OnClassLoadoutUIOpenedEvent();

        public event OnClassLoadoutUIOpenedEvent OnClassLoadoutUIOpened;

        public bool uiOpened = false;

        Dictionary<int, string> troopIndexToTypeTeam1 = new Dictionary<int, string>();
        Dictionary<int, string> troopIndexToTypeTeam2 = new Dictionary<int, string>();

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
            troopTypePercent.Add(new TextObject("{=1Bm1Wk1v}Infantry").ToString(), infCap);
            troopTypePercent.Add(new TextObject("{=rangedtroop}Ranged").ToString(), rangeCap);
            troopTypePercent.Add(new TextObject("{=YVGtcLHF}Cavalry").ToString(), cavCap);
        }

        public void Setup()
        {
            troopIndexToTypeTeam1 = PopulateTroopIndexToTypeDictionary(MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam1.GetStrValue())).ToList());
            troopIndexToTypeTeam2 = PopulateTroopIndexToTypeDictionary(MultiplayerClassDivisions.GetMPHeroClasses(MBObjectManager.Instance.GetObject<BasicCultureObject>(MultiplayerOptions.OptionType.CultureTeam2.GetStrValue())).ToList());
        }

        private Dictionary<int, string> PopulateTroopIndexToTypeDictionary(List<MultiplayerClassDivisions.MPHeroClass> classes)
        {
            Dictionary<int, string> toReturn = new Dictionary<int, string>();
            int index = 0;
            foreach (var troopClass in classes)
            {
                toReturn.Add(index, troopClass.ClassGroup.Name.ToString());
                index++;
            }

            return toReturn;
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
            troopTypePercent[new TextObject("{=1Bm1Wk1v}Infantry").ToString()] = infCap;
            troopTypePercent[new TextObject("{=rangedtroop}Ranged").ToString()] = rangeCap;
            troopTypePercent[new TextObject("{=YVGtcLHF}Cavalry").ToString()] = cavCap;
        }

        public void RefreshLoadoutVM()
        {
            if(_vm != null)
            {
                ResetVM();
                Dictionary<string, float> currentTroopBreakdown = TroopCapLogic.GetCurrentTeamClassTypeBreakdown(PlayerWrapper.GetMyTeamTroopIndeces(),GetTroopIndexToTroopTypeDictionary(_vm.Classes.ToList()), troopTypePercent.Keys.ToList());
                Dictionary<string, bool> classIsAvailable = TroopCapLogic.GetTroopClassAvailabilityDictionary(currentTroopBreakdown, troopTypePercent);
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
