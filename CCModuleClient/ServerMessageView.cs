using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace CCModuleClient
{
    class ServerMessageView : MissionView
    {

        ServerMessageVM _dataSource;
        GauntletLayer _layer;
        IGauntletMovie _movie;

        public ServerMessageView()
        {
            
        }

        public static void UpdateServerMessage(string message)
        {
            if (Mission.Current != null)
            {
                ServerMessageView serverMessageView = Mission.Current.GetMissionBehavior<ServerMessageView>();
                if (serverMessageView != null && serverMessageView._dataSource != null)
                {
                    Thread t = new Thread(new ParameterizedThreadStart(serverMessageView.UpdateThread));
                    t.Start(message);
                }
            }
        }

        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();
            OpenUI();
        }

        public void OpenUI()
        {
            if(MissionScreen != null)
            {
                this.ViewOrderPriority = int.MaxValue - 1;

                _dataSource = new ServerMessageVM("");

                _layer = new GauntletLayer(this.ViewOrderPriority);

                _movie = _layer.LoadMovie("ServerMessage", _dataSource);

                MissionScreen.AddLayer(_layer);
            }
        }


        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();
            End();
        }

        public void End()
        {
            if(MissionScreen != null && _layer != null)
            {
                MissionScreen.RemoveLayer(_layer);
                _layer = null;
                _movie = null;
                _dataSource = null;
            }
            
        }

        public override void OnRemoveBehavior()
        {
            if(MissionScreen != null && _layer != null)
            {
                MissionScreen.RemoveLayer(_layer);
                _layer = null;
                _movie = null;
                _dataSource = null;
            }
        }

        void UpdateThread(object obj)
        {
            if(_dataSource != null)
            {
                string message = (string)obj;
                _dataSource.ServerMessage = message;
                Thread.Sleep(3000);
                if (_dataSource != null)
                {
                    _dataSource.ServerMessage = "";
                } 
            }
        }
    }

    class ServerMessageVM : ViewModel
    {
        private string _message;
        public string ServerMessage
        {
            get
            {
                return _message;
            }
            set
            {
                _message = value;
                base.OnPropertyChangedWithValue(value, "ServerMessage");

            }
        }

        public ServerMessageVM(string serverMessage)
        {
            ServerMessage = serverMessage;
        }
    }
}
