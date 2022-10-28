using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.MountAndBlade;

namespace CCModuleClient
{
    class PlayerWrapper
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
                if(peer != GameNetwork.MyPeer || includeMyself)
                {
                    MissionPeer mp = peer.GetComponent<MissionPeer>();
                    toReturn.Add(mp);
                }
            }

            return toReturn;
        }

        public static List<int> GetMyTeamTroopIndeces(bool includeMyself = false)
        {
            List<int> toReturn = new List<int>();

            MissionPeer myMP = GetMyMissionPeer();
            if (myMP != null && myMP.Team != null)
            {
                TaleWorlds.Core.BattleSideEnum mySide = myMP.Team.Side;
                foreach (var peer in GameNetwork.NetworkPeers)
                {
                    if (peer != GameNetwork.MyPeer || includeMyself)
                    {
                        MissionPeer mp = peer.GetComponent<MissionPeer>();
                        if (mp != null && mp.Team != null && mp.Team.Side == mySide)
                        {
                            toReturn.Add(mp.SelectedTroopIndex);
                        }
                    }
                }
            }

            return toReturn;
        }
    }
}
