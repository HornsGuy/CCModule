
using CCModuleNetworkMessages.FromClient;
using CCModuleNetworkMessages.FromServer;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleClient
{
    class AdminPanelNetworkMessagesClient : MissionNetwork
    {
        public AdminPanelNetworkMessagesClient()
        {
            OnAfterMissionCreated();
        }

        protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
        {
            ChatMessageManager.AddMessage("Registered Messages", 55, 189, 40);
            base.AddRemoveMessageHandlers(registerer);
            registerer.Register<AdminLoginMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<AdminLoginMessage>(this.HandleAdminLoginMessage));
        }

        private bool HandleAdminLoginMessage(NetworkCommunicator peer, AdminLoginMessage message)
        {
            ChatMessageManager.AddMessage("Admin Login", 55, 189, 40);
            CCModuleClientSubModule.playerIsAdmin = true;
            return true;
        }

    }
}
