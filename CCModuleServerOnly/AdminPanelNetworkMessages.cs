
using CCModuleNetworkMessages.FromClient;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;

namespace CCModuleServerOnly
{
    class AdminPanelNetworkMessages : MissionNetwork
    {
        public AdminPanelNetworkMessages()
        {
            // Have to call here because 
            OnAfterMissionCreated();
        }

        protected override void AddRemoveMessageHandlers(GameNetwork.NetworkMessageHandlerRegistererContainer registerer)
        {
            base.AddRemoveMessageHandlers(registerer);
            Debug.Print("Registed My MEssages", 0, Debug.DebugColor.Magenta);
            registerer.Register<APEndWarmupMessage>(new GameNetworkMessage.ClientMessageHandlerDelegate<APEndWarmupMessage>(this.HandleClientEventRequestCultureChange));
        }

        private bool HandleClientEventRequestCultureChange(NetworkCommunicator peer, APEndWarmupMessage message)
        {
            Debug.Print("!!!Received Message!!!",0,Debug.DebugColor.Yellow);
            return true;
        }

    }
}
