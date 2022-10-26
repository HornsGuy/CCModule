
using CCModuleNetworkMessages.FromClient;
using CCModuleNetworkMessages.FromServer;
using NetworkMessages.FromClient;
using NetworkMessages.FromServer;
using System.Collections.Generic;
using System.Threading;
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
            handlerRegisterer.Register<RequestTroopIndexChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestTroopIndexChange>(this.HandleRequestTroopIndexChange));
            handlerRegisterer.Register<OpenAdminPanelMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<OpenAdminPanelMessage>(this.HandleOpenAdminPanelMessage));
            handlerRegisterer.Register<RequestMapsForGameType>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestMapsForGameType>(this.HandleRequestMapsForGameType));
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
            GameNetwork.WriteMessage(AdminPanelData.Instance.CreateSyncMessage(false));
            GameNetwork.EndModuleEventAsServer();
        }

        private void SyncTroopCapWithClients()
        {
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage(new TroopCapServerMessage(AdminPanelData.Instance.InfantryCap, AdminPanelData.Instance.RangedCap, AdminPanelData.Instance.CavalryCap, AdminPanelData.Instance.HorseArcherCap,true));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None);
        }

        private bool CheckPeerIsAdminBanOtherwise(NetworkCommunicator peer)
        {
            if(PlayerManager.Instance.PlayerIsAdmin(peer.VirtualPlayer.Id.ToString()))
            {
                return true;
            }
            else
            {
                // Ban the person using a hacked client
                PlayerManager.Instance.BanPlayer(peer.VirtualPlayer.UserName, peer.VirtualPlayer.Id.ToString());
                AdminPanel.Instance.KickPlayer(peer);
                return false;
            }
        }

        private bool HandleUpdateTroopCapMessage(NetworkCommunicator peer, APUpdateTroopCapMessage message)
        {
            if(CheckPeerIsAdminBanOtherwise(peer) && AdminPanelData.Instance.UpdateTroopCapsIfDifferent(message.InfantryCap, message.RangedCap, message.CavalryCap, message.HorseArcherCap))
            {
                SyncTroopCapWithClients();
            }
            
            return true;
        }

        private void SetSelectedTroopIndexThread(object peerObject)
        {
            Thread.Sleep(10);
            NetworkCommunicator peer = (NetworkCommunicator)peerObject;
            MissionPeer missionPeer = peer.GetComponent<MissionPeer>();
            if(missionPeer != null)
            {
                missionPeer.SelectedTroopIndex = 0;
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage((GameNetworkMessage)new UpdateSelectedTroopIndex(peer, missionPeer.SelectedTroopIndex));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, peer);
            }
        }

        private bool HandleRequestTroopIndexChange(NetworkCommunicator peer, RequestTroopIndexChange message)
        {
            // Get what type of troop the peer is changing to and see if they are exceeding the troop cap
            MissionPeer missionPeer = peer.GetComponent<MissionPeer>();
            if (!TroopCapServerLogic.Instance.CheckIfPlayerTroopIndexIsUnderCap(missionPeer, message.SelectedTroopIndex))
            {
                Thread t = new Thread(new ParameterizedThreadStart(SetSelectedTroopIndexThread));
                t.Start(peer);
            }
            return true;
        }
        
        private bool HandleRequestMapsForGameType(NetworkCommunicator peer, RequestMapsForGameType message)
        {
            List<string> maps = AdminPanel.Instance.GetMapsForGameType(message.GameType);

            GameNetwork.BeginModuleEventAsServer(peer);
            GameNetwork.WriteMessage(new ReturnMapsForGameTypeMessage(maps));
            GameNetwork.EndModuleEventAsServer();
            return true;
        }
        
        private bool HandleOpenAdminPanelMessage(NetworkCommunicator peer, OpenAdminPanelMessage message)
        {
            // Turn around and send the player the admin panel data
            GameNetwork.BeginModuleEventAsServer(peer);
            GameNetwork.WriteMessage(AdminPanelData.Instance.CreateSyncMessage(false));
            GameNetwork.EndModuleEventAsServer();

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
