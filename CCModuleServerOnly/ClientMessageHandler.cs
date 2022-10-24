
using CCModuleNetworkMessages.FromClient;
using CCModuleNetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace CCModuleServerOnly
{
    class ClientMessageHandler : GameHandler
    {

        protected override void OnGameNetworkBegin()
        {
            base.OnGameNetworkBegin();
            this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);
        }

        protected override void OnGameNetworkEnd()
        {
            base.OnGameNetworkEnd();
            this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
        }

        private void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
        {
            Debug.Print("Listening for client messages", 0, Debug.DebugColor.Magenta);
            GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
            handlerRegisterer.Register<APEndWarmupMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<APEndWarmupMessage>(this.HandleEndWarmupMessage));
            handlerRegisterer.Register<ClientListeningMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<ClientListeningMessage>(this.HandleClientListeningMessage));
            handlerRegisterer.Register<APUpdateTroopCapMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<APUpdateTroopCapMessage>(this.HandleUpdateTroopCapMessage));
        }

        private bool HandleEndWarmupMessage(NetworkCommunicator peer, APEndWarmupMessage message)
        {
            Debug.Print("Temp Message: HandleEndWarmupMessage", 0, Debug.DebugColor.Magenta);

            return true;
        }

        private bool HandleClientListeningMessage(NetworkCommunicator peer, ClientListeningMessage message)
        {
            GameNetwork.BeginModuleEventAsServer(peer);
            GameNetwork.WriteMessage(new AdminLoginMessage());
            GameNetwork.EndModuleEventAsServer();
            SyncAdminPanelSettingsWithClients(peer);
            return true;
        }

        private void SyncAdminPanelSettingsWithClients(NetworkCommunicator peer)
        {
            GameNetwork.BeginModuleEventAsServer(peer);
            GameNetwork.WriteMessage(new TroopCapServerMessage(AdminPanelData.Instance.InfantryCap, AdminPanelData.Instance.RangedCap, AdminPanelData.Instance.CavalryCap));
            GameNetwork.EndModuleEventAsServer();
        }

        private void SyncTroopCapWithClients()
        {
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage(new TroopCapServerMessage(AdminPanelData.Instance.InfantryCap, AdminPanelData.Instance.RangedCap, AdminPanelData.Instance.CavalryCap));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
        }

        private bool HandleUpdateTroopCapMessage(NetworkCommunicator peer, APUpdateTroopCapMessage message)
        {
            if(AdminPanelData.Instance.UpdateTroopCapsIfDifferent(message.InfantryCap, message.RangedCap, message.CavalryCap))
            {
                SyncTroopCapWithClients();
            }
            
            return true;
        }

        public override void OnAfterSave()
        {
            throw new System.NotImplementedException();
        }

        public override void OnBeforeSave()
        {
            throw new System.NotImplementedException();
        }


    }
}
