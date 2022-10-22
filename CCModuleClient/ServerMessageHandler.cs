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

            // Now that we listening for messages, ask the server to see if we are an admin
            GameNetwork.BeginModuleEventAsClient();
            GameNetwork.WriteMessage(new OpenAdminPanelMessage());
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
        }

        private void HandleAdminLoginMessage(AdminLoginMessage message)
        {
            ChatMessageManager.ServerMessage("Logged In");
            CCModuleClientSubModule.playerIsAdmin = true;
        }


        public override void OnAfterSave()
        {

        }

        public override void OnBeforeSave()
        {

        }
    }
}
