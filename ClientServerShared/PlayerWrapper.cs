using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace ClientServerShared
{
    public class PlayerWrapper
    {
        public static MissionPeer GetMyMissionPeer()
        {
            return GameNetwork.MyPeer.GetComponent<MissionPeer>();
        }

        public static List<MissionPeer> GetMissionPeers(bool includeMyself = false)
        {
            List<MissionPeer> toReturn = new List<MissionPeer>();

            foreach (var peer in GameNetwork.NetworkPeers)
            {
                if (peer != GameNetwork.MyPeer || includeMyself)
                {
                    MissionPeer mp = peer.GetComponent<MissionPeer>();
                    if (mp != null)
                    {
                        toReturn.Add(mp);
                    }
                }
            }

            return toReturn;
        }

        public static MissionPeer GetMissionPeerFromNetworkPeer(NetworkCommunicator netPeer)
        {
            return netPeer.GetComponent<MissionPeer>();
        }

        public static List<int> GetPeerTeamTroopIndeces(MissionPeer currentPeer, bool includePeer = false)
        {
            List<int> toReturn = new List<int>();

            if (currentPeer != null)
            {
                foreach (var peer in GameNetwork.NetworkPeers)
                {
                    MissionPeer mp = peer.GetComponent<MissionPeer>();
                    if (mp != null && (currentPeer != mp || includePeer) && mp.Team != currentPeer.Team)
                    {
                        toReturn.Add(mp.SelectedTroopIndex);
                    }
                }
            }

            return toReturn;
        }
    }
}
