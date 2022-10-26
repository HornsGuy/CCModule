﻿using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.ScreenSystem;
using TaleWorlds.Core.ViewModelCollection.Selector;
using System.Collections.Generic;
using System;
using TaleWorlds.MountAndBlade.Network.Messages;
using CCModuleNetworkMessages.FromClient;

namespace CCModuleClient
{

    public class AdminPanelMissionView : MissionView 
    {

        GauntletLayer _layer;
        IGauntletMovie _movie;
        AdminPanelVM _dataSource;

        public AdminPanelMissionView()
        {
            this.ViewOrderPriority = 10;
        }

        public static void UpdateAvailableMaps(List<string> maps)
        {
            if (Mission.Current != null)
            {
                AdminPanelMissionView adminPanelMissionView = Mission.Current.GetMissionBehavior<AdminPanelMissionView>();
                if (adminPanelMissionView != null && adminPanelMissionView._dataSource != null)
                {
                    adminPanelMissionView._dataSource.UpdateAvailableMaps(maps);
                }
            }
        }

        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();
            CloseAdminPanelUI();
        }

        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);

            
            if (Input.IsKeyPressed(TaleWorlds.InputSystem.InputKey.F8))
            {
                if(CCModuleClientSubModule.playerIsAdmin)
                {
                    if (!isPanelOpen())
                    {
                        OpenAdminPanelUI();
                    }
                    else
                    {
                        CloseAdminPanelUI();
                    }
                }
            }

            if(_dataSource!=null)
            {
                if (_dataSource.cancelPressed)
                {
                    CloseAdminPanelUI();
                }
            }
        }

        private bool isPanelOpen()
        {
            return _layer != null;
        }

        private void OpenAdminPanelUI()
        {
            // Message server to get fresh data
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new OpenAdminPanelMessage());
            GameNetwork.EndModuleEventAsClient();
            
            _dataSource = new AdminPanelVM();
            _layer = new GauntletLayer(this.ViewOrderPriority);
            
            
            _movie = _layer.LoadMovie("CCAdminPanel", _dataSource);
            _layer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.Mouse | InputUsageMask.MouseWheels);
            MissionScreen.AddLayer(_layer);
        }

        private void CloseAdminPanelUI()
        {
            if(isPanelOpen())
            {
                MissionScreen.RemoveLayer(_layer);
                _layer = null;
                _movie = null;
                _dataSource = null;
            }
        }
    }

    class AdminPanelVM : ViewModel
    {
        private SelectorVM<SelectorItemVM> _gameTypes;
        private SelectorVM<SelectorItemVM> _maps;
        private SelectorVM<SelectorItemVM> _factions1;
        private SelectorVM<SelectorItemVM> _factions2;
        private int _mapTimeInMinutes = 7;
        private int _roundTimeInMinutes = 8;
        private int _warmupTimeInMinutes = 9;

        private int _infClassCap = 78;
        private int _archerClassCap = 79;
        private int _cavClassCap = 80;
        private int _haClassCap = 81;

        public bool cancelPressed = false;

        private List<string> gameModes = new List<string>();

        private List<string> maps = new List<string>();

        private List<string> factionStrings = new List<string>()
        {
            "Aserai",
            "Battania",
            "Empire",
            "Sturgia",
            "Kuzait",
            "Vlandia"
        };

        public AdminPanelVM()
        {
            // Game Mode
            gameModes = MultiplayerOptions.Instance.GetMultiplayerOptionsList(MultiplayerOptions.OptionType.GameType);
            
            string tempGameType = "";
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.GameType).GetValue(out tempGameType);
            int selectedIndex = gameModes.IndexOf(tempGameType);
            
            this.GameTypes = new SelectorVM<SelectorItemVM>(gameModes, selectedIndex, new Action<SelectorVM<SelectorItemVM>>(this.OnGameTypeChanged));

            // Maps
            maps = AdminPanelClientData.Instance.AvailableMaps;
            this.Maps = new SelectorVM<SelectorItemVM>(maps, 0, new Action<SelectorVM<SelectorItemVM>>(this.OnMapChanged));

            // Factions
            factionStrings = MultiplayerOptions.Instance.GetMultiplayerOptionsList(MultiplayerOptions.OptionType.CultureTeam1);
            
            string tempFaction1 = "";
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam1).GetValue(out tempFaction1);
            selectedIndex = factionStrings.IndexOf(tempFaction1);

            this.Faction1 = new SelectorVM<SelectorItemVM>(factionStrings, 0, new Action<SelectorVM<SelectorItemVM>>(this.OnFaction1Changed));

            string tempFaction2 = "";
            MultiplayerOptions.Instance.GetOptionFromOptionType(MultiplayerOptions.OptionType.CultureTeam1).GetValue(out tempFaction2);
            selectedIndex = factionStrings.IndexOf(tempFaction2);

            this.Faction2 = new SelectorVM<SelectorItemVM>(factionStrings, 0, new Action<SelectorVM<SelectorItemVM>>(this.OnFaction2Changed));

            // Timers


            // Troop Caps
            _infClassCap = AdminPanelClientData.Instance.InfantryCap;
            _archerClassCap = AdminPanelClientData.Instance.RangedCap;
            _cavClassCap = AdminPanelClientData.Instance.CavalryCap;
            _haClassCap = AdminPanelClientData.Instance.HorseArcherCap;
        }

        public void UpdateAvailableMaps(List<string> maps)
        {
            this.maps = maps;
        }

        private void OnGameTypeChanged(SelectorVM<SelectorItemVM> obj)
        {
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new RequestMapsForGameType(obj.SelectedItem.StringItem));
            GameNetwork.EndModuleEventAsClient();
        }

        private void OnMapChanged(SelectorVM<SelectorItemVM> obj)
        {

        }

        private void OnFaction1Changed(SelectorVM<SelectorItemVM> obj)
        {
        }

        private void OnFaction2Changed(SelectorVM<SelectorItemVM> obj)
        {
        }

        private void OnMapTimeChanged()
        {
            // Tell server to change map time
        }
        
        private void OnRoundTimeChanged()
        {
            // Tell server to change round time
        }

        private void OnWarmupTimeChanged()
        {
            // Tell server to change warmup time
        }

        private async void InfantryCapFocusLost()
        {
            SendTroopCapUpdateMessage();
        }

        private async void RangeCapFocusLost()
        {
            SendTroopCapUpdateMessage();
        }
        
        private async void CavCapFocusLost()
        {
            SendTroopCapUpdateMessage();
        }

        private async void HorseArcherCapFocusLost()
        {
            SendTroopCapUpdateMessage();
        }

        private void SendTroopCapUpdateMessage()
        {
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new APUpdateTroopCapMessage(InfantryCapPercentage, ArcherCapPercentage, CavCapPercentage, HorseArcherCapPercentage));
            GameNetwork.EndModuleEventAsClient();
        }

        private async void ExecuteDone()
        {
            // Starts mission
        }
        
        private async void ExecuteCancel()
        {
            cancelPressed = true;
        }

        private async void ExecuteEndWarmup()
        {
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new APEndWarmupMessage());
            GameNetwork.EndModuleEventAsClient();
        }

        [DataSourceProperty]
        public SelectorVM<SelectorItemVM> GameTypes
        {
            get
            {
                return this._gameTypes;
            }
            set
            {
                if (value != this._gameTypes)
                {
                    this._gameTypes = value;
                    base.OnPropertyChangedWithValue(value, "GameTypes");
                }
            }
        }

        [DataSourceProperty]
        public SelectorVM<SelectorItemVM> Maps
        {
            get
            {
                return this._maps;
            }
            set
            {
                if (value != this._maps)
                {
                    this._maps = value;
                    base.OnPropertyChangedWithValue(value, "Maps");
                }
            }
        }

        [DataSourceProperty]
        public SelectorVM<SelectorItemVM> Faction1
        {
            get
            {
                return this._factions1;
            }
            set
            {
                if (value != this._factions1)
                {
                    this._factions1 = value;
                    base.OnPropertyChangedWithValue(value, "Faction1");
                }
            }
        }

        [DataSourceProperty]
        public SelectorVM<SelectorItemVM> Faction2
        {
            get
            {
                return this._factions2;
            }
            set
            {
                if (value != this._factions2)
                {
                    this._factions2 = value;
                    base.OnPropertyChangedWithValue(value, "Faction2");
                }
            }
        }

        [DataSourceProperty]
        public int MapTime
        {
            get
            {
                return this._mapTimeInMinutes;
            }
            set
            {
                if (value != this._mapTimeInMinutes)
                {
                    this._mapTimeInMinutes = value;
                    base.OnPropertyChangedWithValue(value, "MapTime");
                    OnMapTimeChanged();
                }
            }
        }

        [DataSourceProperty]
        public int RoundTime
        {
            get
            {
                return this._roundTimeInMinutes;
            }
            set
            {
                if (value != this._roundTimeInMinutes)
                {
                    _roundTimeInMinutes = value;
                    base.OnPropertyChangedWithValue(value, "RoundTime");
                    OnRoundTimeChanged();
                }
            }
        }

        [DataSourceProperty]
        public int WarmupTime
        {
            get
            {
                return this._warmupTimeInMinutes;
            }
            set
            {
                if (value != this._warmupTimeInMinutes)
                {
                    _warmupTimeInMinutes = value;
                    base.OnPropertyChangedWithValue(value, "WarmupTime");
                    OnWarmupTimeChanged();
                }
            }
        }

        [DataSourceProperty]
        public int InfantryCapPercentage
        {
            get
            {
                return this._infClassCap;
            }
            set
            {
                if (value != this._infClassCap)
                {
                    _infClassCap = value;
                    base.OnPropertyChangedWithValue(value, "InfantryCapPercentage");
                }
            }
        }

        [DataSourceProperty]
        public int ArcherCapPercentage
        {
            get
            {
                return this._archerClassCap;
            }
            set
            {
                if (value != this._archerClassCap)
                {
                    _archerClassCap = value;
                    base.OnPropertyChangedWithValue(value, "ArcherCapPercentage");
                }
            }
        }

        [DataSourceProperty]
        public int CavCapPercentage
        {
            get
            {
                return this._cavClassCap;
            }
            set
            {
                if (value != this._cavClassCap)
                {
                    _cavClassCap = value;
                    base.OnPropertyChangedWithValue(value, "CavCapPercentage");
                }
            }
        }

        [DataSourceProperty]
        public int HorseArcherCapPercentage
        {
            get
            {
                return this._haClassCap;
            }
            set
            {
                if (value != this._haClassCap)
                {
                    _haClassCap = value;
                    base.OnPropertyChangedWithValue(value, "HorseArcherCapPercentage");
                }
            }
        }
    }
}
