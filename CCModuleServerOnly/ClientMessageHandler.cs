
using CCModuleNetworkMessages.FromClient;
using CCModuleNetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

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
            Debug.Print("Registered Messaged", 0, Debug.DebugColor.Magenta);
            GameNetwork.NetworkMessageHandlerRegisterer handlerRegisterer = new GameNetwork.NetworkMessageHandlerRegisterer(mode);
            handlerRegisterer.Register<APEndWarmupMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<APEndWarmupMessage>(this.HandleEndWarmupMessage));
            handlerRegisterer.Register<OpenAdminPanelMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<OpenAdminPanelMessage>(this.HandleOpenAdminPanelMessage));
        }

        private bool HandleEndWarmupMessage(NetworkCommunicator peer, APEndWarmupMessage message)
        {
            Debug.Print("End Warmup Message",0,Debug.DebugColor.Yellow);
            return true;
        }

        private bool HandleOpenAdminPanelMessage(NetworkCommunicator peer, OpenAdminPanelMessage message)
        {
            Debug.Print("!!!Received Message!!!",0,Debug.DebugColor.Yellow);
            GameNetwork.BeginModuleEventAsServer(peer);
            GameNetwork.WriteMessage(new AdminLoginMessage());
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
