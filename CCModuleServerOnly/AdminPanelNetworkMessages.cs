
using CCModuleNetworkMessages.FromClient;
using CCModuleNetworkMessages.FromServer;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleServerOnly
{
    class AdminPanelNetworkMessages : MissionNetwork
    {
        public AdminPanelNetworkMessages()
        {
            OnAfterMissionCreated();
        }

        protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
        {
            Debug.Print("Registered Messaged", 0, Debug.DebugColor.Magenta);
            base.AddRemoveMessageHandlers(registerer);
            registerer.Register<APEndWarmupMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<APEndWarmupMessage>(this.HandleEndWarmupMessage));
            registerer.Register<OpenAdminPanelMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<OpenAdminPanelMessage>(this.HandleOpenAdminPanelMessage));
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

    }
}
