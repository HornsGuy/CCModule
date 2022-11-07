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

        // TODO: Should really refactor this code out into its own type of thread implmentation
        const int defaultMessageTimeoutInTenths = 20;
        static int timeUntilMessageDisappearsInTenths = 0;

        static int threadRunning = 0;
        static bool ThreadNotRunning
        {
            get
            {
                return threadRunning == 0;
            }
        }
        
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
                    // We have a message to display
                    if(message != "")
                    {
                        // Whether thread is running or not, we reset the timer.
                        // This will extend the original message, or setup for a new one
                        Interlocked.Exchange(ref timeUntilMessageDisappearsInTenths, defaultMessageTimeoutInTenths);

                        if (ThreadNotRunning)
                        {
                            // Thread is not running, so start a new one 
                            Interlocked.Increment(ref threadRunning);
                            Thread t = new Thread(new ParameterizedThreadStart(serverMessageView.MessageFadeThread));
                            t.Start(message);
                        }
                    }
                    else
                    {
                        // We need to clear message, so just set the timer to 1 so it ends organically
                        Interlocked.Exchange(ref timeUntilMessageDisappearsInTenths, 1);
                    }
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

        void MessageFadeThread(object obj)
        {
            if (_dataSource != null)
            {
                string message = (string)obj;
                _dataSource.ServerMessage = message;
                
                while(timeUntilMessageDisappearsInTenths > 0)
                {
                    Thread.Sleep(100);
                    Interlocked.Decrement(ref timeUntilMessageDisappearsInTenths);
                }

                if (_dataSource != null)
                {
                    _dataSource.ServerMessage = "";
                } 
            }
            Interlocked.Decrement(ref threadRunning);
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
