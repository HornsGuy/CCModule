
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
            handlerRegisterer.Register<RequestTroopIndexChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestTroopIndexChange>(this.HandleRequestTroopIndexChange));
            handlerRegisterer.Register<OpenAdminPanelMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<OpenAdminPanelMessage>(this.HandleOpenAdminPanelMessage));
            handlerRegisterer.Register<RequestMapsForGameType>(new GameNetworkMessage.ClientMessageHandlerDelegate<RequestMapsForGameType>(this.HandleRequestMapsForGameType));
            handlerRegisterer.Register<APStartMissionMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<APStartMissionMessage>(this.HandleAPStartMissionMessage));
            handlerRegisterer.Register<TeamChange>(new GameNetworkMessage.ClientMessageHandlerDelegate<TeamChange>(this.HandleTeamChange));
            handlerRegisterer.Register<AgentSpawnedMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<AgentSpawnedMessage>(this.HandleAgentSpawnedMessage));
            handlerRegisterer.Register<ReEquipInitialWeapons>(new GameNetworkMessage.ClientMessageHandlerDelegate<ReEquipInitialWeapons>(this.HandleReEquipInitialWeapons));

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

        private void SetSelectedTroopIndexThread(object peerObject)
        {
            Thread.Sleep(10);
            NetworkCommunicator peer = (NetworkCommunicator)peerObject;
            MissionPeer missionPeer = peer.GetComponent<MissionPeer>();
            if(missionPeer != null)
            {
                missionPeer.SelectedTroopIndex = 0;
                PlayerWrapper.Instance.SetTroopIndexForPlayer(peer.VirtualPlayer.Id.ToString(), 0);
                GameNetwork.BeginBroadcastModuleEvent();
                GameNetwork.WriteMessage((GameNetworkMessage)new UpdateSelectedTroopIndex(peer, missionPeer.SelectedTroopIndex));
                GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.ExcludeOtherTeamPlayers, peer);
            }
        }

        private bool HandleRequestTroopIndexChange(NetworkCommunicator peer, RequestTroopIndexChange message)
        {
            // Get what type of troop the peer is changing to and see if they are exceeding the troop cap
            // This is just in case of a hacked client
            if (!TroopCapServerLogic.Instance.CheckIfPlayerTroopIndexIsUnderCap(peer.VirtualPlayer.Id.ToString(), message.SelectedTroopIndex))
            {
                Thread t = new Thread(new ParameterizedThreadStart(SetSelectedTroopIndexThread));
                t.Start(peer);
            }
            else
            {
                PlayerWrapper.Instance.SetTroopIndexForPlayer(peer.VirtualPlayer.Id.ToString(), message.SelectedTroopIndex);
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
        
        private bool HandleAgentSpawnedMessage(NetworkCommunicator peer, AgentSpawnedMessage message)
        {
            if(message.peer.VirtualPlayer != null && EquipmentOverride.Instance.PlayerHasEquipmentToBeOverridden(message.peer.VirtualPlayer.Id.ToString()))
            {
                
                Equipment newEquipment = EquipmentOverride.Instance.GetOverriddenEquipment(message.peer.VirtualPlayer.Id.ToString(), message.peer.ControlledAgent);
                GameNetwork.BeginModuleEventAsServer(peer);
                GameNetwork.WriteMessage(new ChangePlayerCosmeticEquipment(message.peer, newEquipment));
                GameNetwork.EndModuleEventAsServer();

            }

            return true;
        }
        
        void ThreadProc(object peer)
        {
            
            NetworkCommunicator netPeer = (NetworkCommunicator)peer;
            if(netPeer != null && netPeer.ControlledAgent != null)
            {
                if (netPeer.ControlledAgent.WieldedWeapon.Item != null)
                {
                    netPeer.ControlledAgent.TryToSheathWeaponInHand(Agent.HandIndex.MainHand, Agent.WeaponWieldActionType.WithAnimation);
                    Debug.Print("Main Hand", 0, Debug.DebugColor.Purple);
                }
                Thread.Sleep(500);
                if(netPeer.ControlledAgent.WieldedOffhandWeapon.Item != null)
                {
                    netPeer.ControlledAgent.TryToSheathWeaponInHand(Agent.HandIndex.OffHand, Agent.WeaponWieldActionType.WithAnimation);
                    Debug.Print("Off Hand", 0, Debug.DebugColor.Purple);
                }
                
            }
        }

        private bool HandleReEquipInitialWeapons(NetworkCommunicator peer, ReEquipInitialWeapons message)
        {
            if(peer.ControlledAgent != null)
            {
                Thread t = new Thread(new ParameterizedThreadStart(ThreadProc));
                t.Start(peer);
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
