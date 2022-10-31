﻿using CCModuleNetworkMessages.FromClient;
using CCModuleNetworkMessages.FromServer;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.ObjectSystem;

namespace CCModuleClient
{
    class ServerMessageHandler : GameHandler
    {

        protected override void OnGameNetworkBegin()
        {
            base.OnGameNetworkBegin();
            this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Add);

            // Inform the server we are now listening for messages
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new ClientListeningMessage());
            GameNetwork.EndModuleEventAsClient();
        }

        protected override void OnGameNetworkEnd()
        {
            base.OnGameNetworkEnd();
            this.AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode.Remove);
        }

        private void AddRemoveMessageHandlers(
      GameNetwork.NetworkMessageHandlerRegisterer.RegisterMode mode)
        {
            GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
            handlerRegisterer.Register<AdminLoginMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<AdminLoginMessage>(this.HandleAdminLoginMessage));
            handlerRegisterer.Register<TroopCapServerMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<TroopCapServerMessage>(this.HandleTroopCapServerMessage));
            handlerRegisterer.Register<SyncAdminPanelMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<SyncAdminPanelMessage>(this.HandleSyncAdminPanelMessage));
            handlerRegisterer.Register<ReturnMapsForGameTypeMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<ReturnMapsForGameTypeMessage>(this.HandleReturnMapsForGameTypeMessage));
            handlerRegisterer.Register<ColoredChatMessage>(new GameNetworkMessage.ServerMessageHandlerDelegate<ColoredChatMessage>(this.HandleColoredChatMessage));
            handlerRegisterer.Register<CreateAgent>(new GameNetworkMessage.ServerMessageHandlerDelegate<CreateAgent>(this.HandleCreateAgent));
            handlerRegisterer.Register<ChangePlayerCosmeticEquipment>(new GameNetworkMessage.ServerMessageHandlerDelegate<ChangePlayerCosmeticEquipment>(this.HandleChangePlayerCosmeticEquipment));
        }

        private void HandleAdminLoginMessage(AdminLoginMessage message)
        {
            ChatMessageManager.ServerMessage("F8 for Admin Panel");
            CCModuleClientSubModule.playerIsAdmin = true;
        }

        private void HandleSyncAdminPanelMessage(SyncAdminPanelMessage message)
        {
            AdminPanelClientData.Instance.Update(message);
        }

        private void HandleTroopCapServerMessage(TroopCapServerMessage message)
        {
            if(AdminPanelClientData.Instance.UpdateTroopCapsIfDifferent(message.InfantryCap, message.RangedCap, message.CavalryCap, message.HorseArcherCap) && message.PrintMessage)
            {
                ChatMessageManager.ServerMessage("Updated Troop Caps:");
                ChatMessageManager.ServerMessage("Inf: " + message.InfantryCap + "%");
                ChatMessageManager.ServerMessage("Range: " + message.RangedCap + "%");
                ChatMessageManager.ServerMessage("Cav: " + message.CavalryCap + "%");
                ChatMessageManager.ServerMessage("HA: " + message.HorseArcherCap + "%");
            }
        }
        
        private void HandleReturnMapsForGameTypeMessage(ReturnMapsForGameTypeMessage message)
        {
            AdminPanelClientData.Instance.UpdateAvailableMaps(message.AvailableMaps);
        }
        
        private void HandleColoredChatMessage(ColoredChatMessage message)
        {
            List<string> lines = new List<string>();
            if (message.Message.Contains("\n"))
            {
                lines = new List<string>(message.Message.Split('\n'));
            }
            else
            {
                lines.Add(message.Message);
            }

            foreach (var line in lines)
            {
                ChatMessageManager.AddMessage(line, message.Red, message.Green, message.Blue);
            }
            
        }
        
        private void HandleCreateAgent(CreateAgent message)
        {
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new AgentSpawnedMessage(message.Peer));
            GameNetwork.EndModuleEventAsClient();
        }
        
        private void HandleChangePlayerCosmeticEquipment(ChangePlayerCosmeticEquipment message)
        {
            NetworkCommunicator peerToUpdateEquipment = message.Peer;
            if(peerToUpdateEquipment != null && peerToUpdateEquipment.ControlledAgent != null && message.Equipment != null)
            {
                Equipment newEquipment = message.Equipment;
                peerToUpdateEquipment.ControlledAgent.UpdateSpawnEquipmentAndRefreshVisuals(newEquipment);

                if(peerToUpdateEquipment == GameNetwork.MyPeer)
                {
                    GameNetwork.BeginModuleEventAsClient();
                    GameNetwork.WriteMessage(new ReEquipInitialWeapons());
                    GameNetwork.EndModuleEventAsClient();
                }
            }
        }

        public override void OnAfterSave()
        {

        }

        public override void OnBeforeSave()
        {

        }
    }
}
