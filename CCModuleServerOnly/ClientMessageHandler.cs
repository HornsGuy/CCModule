
using BannerlordWrapper;
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
using static BannerlordWrapper.TeamWrapper;

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
            handlerRegisterer.Register<OpenAdminPanelMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<OpenAdminPanelMessage>(this.HandleOpenAdminPanelMessage));
            handlerRegisterer.Register<RequestMapsForGameType>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestMapsForGameType>(this.HandleRequestMapsForGameType));
            handlerRegisterer.Register<APStartMissionMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<APStartMissionMessage>(this.HandleAPStartMissionMessage));
            handlerRegisterer.Register<TeamChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<TeamChange>(this.HandleTeamChange));

        }

        private bool HandleEndWarmupMessage(NetworkCommunicator peer, APEndWarmupMessage message)
        {
            Debug.Print("Temp Message: HandleEndWarmupMessage", 0, Debug.DebugColor.Magenta);

            return true;
        }

        private bool HandleClientListeningMessage(NetworkCommunicator peer, ClientListeningMessage message)
        {
            if(PlayerManager.Instance.PlayerIsAdmin(peer.VirtualPlayer.Id.ToString()))
            {
                GameNetwork.BeginModuleEventAsServer(peer);
                GameNetwork.WriteMessage(new AdminLoginMessage());
                GameNetwork.EndModuleEventAsServer();
            }
            SyncAdminPanelSettingsWithClients(peer);
            return true;
        }

        private void SyncAdminPanelSettingsWithClients(NetworkCommunicator peer)
        {
            GameNetwork.BeginModuleEventAsServer(peer);
            GameNetwork.WriteMessage(AdminPanelServerData.Instance.CreateSyncMessage(false));
            GameNetwork.EndModuleEventAsServer();
        }

        private void SyncTroopCapWithClients()
        {
            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage(AdminPanelServerData.Instance.CreateTroopCapServerMessage(true));
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
                AdminPanel.Instance.SendServerMessageToPeer(peer, "You have been banned from the server.");
                AdminPanel.Instance.KickPlayer(peer);
                return false;
            }
        }

        private bool HandleUpdateTroopCapMessage(NetworkCommunicator peer, APUpdateTroopCapMessage message)
        {
            if(CheckPeerIsAdminBanOtherwise(peer) && AdminPanelServerData.Instance.UpdateTroopCapsIfDifferent(message.InfantryCap, message.RangedCap, message.CavalryCap, message.HorseArcherCap))
            {
                SyncTroopCapWithClients();
                TroopCapServerLogic.Instance.OnTroopCapChange();
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
            GameNetwork.WriteMessage(AdminPanelServerData.Instance.CreateSyncMessage(false));
            GameNetwork.EndModuleEventAsServer();

            return true;
        }
        
        private bool HandleAPStartMissionMessage(NetworkCommunicator peer, APStartMissionMessage message)
        {

            if (CheckPeerIsAdminBanOtherwise(peer))
            {
                if (!AdminPanel.Instance.EndingCurrentMissionThenStartingNewMission)
                {
                    string startMissionMessage = "Changing Map:" +
                                            "\nGame Type: " + message.GameType +
                                            "\nMap: " + message.Map +
                                            "\nFaction1: " + message.Faction1 +
                                            "\nFaction2: " + message.Faction2;
                    AdminPanel.Instance.BroadcastServerMessage(startMissionMessage);
                    AdminPanel.Instance.StartMission(message.GameType, message.Map, message.Faction1, message.Faction2);
                }
                else
                {
                    AdminPanel.Instance.SendServerMessageToPeer(peer, "Mission is already being changed.");
                }
            }

            return true;
        }
        
        private bool HandleTeamChange(NetworkCommunicator peer, TeamChange message)
        {
            if(message.Team != null)
            {
                TeamType teamType = TeamType.Spectator;
                if (message.Team.Side == BattleSideEnum.Attacker)
                {
                    teamType = TeamType.Attacker;
                }
                else if(message.Team.Side == BattleSideEnum.Defender)
                {
                    teamType = TeamType.Defender;
                }
                PlayerWrapper.Instance.SetPlayerTeam(peer.VirtualPlayer.Id.ToString(), teamType);
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
