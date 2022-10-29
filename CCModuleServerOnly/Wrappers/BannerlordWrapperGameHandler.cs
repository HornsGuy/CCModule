using NetworkMessages.FromClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Network.Messages;
using TaleWorlds.MountAndBlade;
using BannerlordWrapper;

namespace CCModuleServerOnly.Wrappers
{
    public class BannerlordWrapperGameHandler : GameHandler
    {
        protected override void OnPlayerConnect(VirtualPlayer peer)
        {
            PlayerWrapper.Instance.AddPlayer(new BannerlordWrapper.Player(peer.Id.ToString(), peer.UserName, TeamWrapper.TeamType.Spectator));
        }

        protected override void OnPlayerDisconnect(VirtualPlayer peer)
        {
            PlayerWrapper.Instance.RemovePlayer(peer.Id.ToString());
        }

        public override void OnAfterSave()
        {

        }

        public override void OnBeforeSave()
        {

        }
    }
}
