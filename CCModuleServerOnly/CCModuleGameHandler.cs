using CCModuleNetworkMessages.FromServer;
using NetworkMessages.FromServer;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.DedicatedCustomServer;

namespace CCModuleServerOnly
{
    class CCModuleGameHandler : GameHandler
    {
        public override void OnAfterSave()
        {
            
        }

        protected override void OnPlayerConnect(VirtualPlayer peer)
        {
            base.OnPlayerConnect(peer);

            // Kick players who have been banned
            if(PlayerManager.Instance.PlayerIsBanned(peer.Id.ToString()))
            {
                AdminPanel.Instance.SendServerMessageToPeer(peer.Id.ToString(), "You have been banned from the server.");
                DedicatedCustomServerSubModule.Instance.DedicatedCustomGameServer.KickPlayer(peer.Id, true);
            }

            // Let player know if they are admin
            if (PlayerManager.Instance.PlayerIsAdmin(peer.Id.ToString()))
            {
                // For whatever reason I can't get the client to listen for messages before this is sent. 
                // Instead we are going to have a message be sent when they try to open the panel

                //GameNetwork.BeginModuleEventAsServer(peer);
                //GameNetwork.WriteMessage(new ServerMessage("Admin Logged In"));
                //GameNetwork.EndModuleEventAsServer();

                //Debug.Print(peer.UserName + " is an admin", 0, Debug.DebugColor.Magenta);

                //GameNetwork.BeginModuleEventAsServer(peer);
                //GameNetwork.WriteMessage(new AdminLoginMessage());
                //GameNetwork.EndModuleEventAsServer();
            }

        }

        public override void OnBeforeSave()
        {
            
        }
    }
}
