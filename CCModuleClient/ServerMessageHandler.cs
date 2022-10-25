using CCModuleNetworkMessages.FromClient;
using CCModuleNetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        }

        private void HandleAdminLoginMessage(AdminLoginMessage message)
        {
            ChatMessageManager.ServerMessage("F8 for Admin Panel");
            CCModuleClientSubModule.playerIsAdmin = true;
        }

        private void HandleTroopCapServerMessage(TroopCapServerMessage message)
        {
            if(AdminPanelClientData.Instance.UpdateTroopCapsIfDifferent(message.InfantryCap, message.RangedCap, message.CavalryCap, message.HorseArcherCap))
            {
                TroopCapBehavior.UpdateTroopCaps(message.InfantryCap, message.RangedCap, message.CavalryCap, message.HorseArcherCap);
                if(message.PrintMessage)
                {
                    ChatMessageManager.ServerMessage("Updated Troop Caps:");
                    ChatMessageManager.ServerMessage("Inf: " + message.InfantryCap + "%");
                    ChatMessageManager.ServerMessage("Range: " + message.RangedCap + "%");
                    ChatMessageManager.ServerMessage("Cav: " + message.CavalryCap + "%");
                    ChatMessageManager.ServerMessage("HA: " + message.HorseArcherCap + "%");
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
