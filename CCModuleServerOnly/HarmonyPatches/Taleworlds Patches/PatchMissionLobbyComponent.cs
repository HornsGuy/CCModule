using BannerlordWrapper;
using HarmonyLib;
using NetworkMessages.FromServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.Network.Messages;


namespace CCModuleServerOnly.HarmonyPatches
{ 
    class PatchMissionLobbyComponent_SendPeerInformationsToPeer
    {
        static bool hitOnce = false;
        public static bool Prefix(MissionLobbyComponent __instance, NetworkCommunicator peer)
        {
            if (!hitOnce)
            {
                Logging.Instance.Info("PatchMissionLobbyComponent_SendPeerInformationsToPeer.Prefix has been hit once");
                hitOnce = true;
            }

            foreach (NetworkCommunicator disconnectedPeer in GameNetwork.NetworkPeersIncludingDisconnectedPeers)
            {
                if(disconnectedPeer != null)
                {
                    bool flag = disconnectedPeer.VirtualPlayer != MBNetwork.VirtualPlayers[disconnectedPeer.VirtualPlayer.Index];
                    if (flag || disconnectedPeer.IsSynchronized || disconnectedPeer.JustReconnecting)
                    {
                        if (peer != null)
                        {
                            MissionPeer component = disconnectedPeer.GetComponent<MissionPeer>();
                            if (component != null)
                            {
                                GameNetwork.BeginModuleEventAsServer(peer);
                                GameNetwork.WriteMessage((GameNetworkMessage)new KillDeathCountChange(component.GetNetworkPeer(), (NetworkCommunicator)null, component.KillCount, component.AssistCount, component.DeathCount, component.Score));
                                GameNetwork.EndModuleEventAsServer();
                                if (component.BotsUnderControlAlive != 0 || component.BotsUnderControlTotal != 0)
                                {
                                    GameNetwork.BeginModuleEventAsServer(peer);
                                    GameNetwork.WriteMessage((GameNetworkMessage)new BotsControlledChange(component.GetNetworkPeer(), component.BotsUnderControlAlive, component.BotsUnderControlTotal));
                                    GameNetwork.EndModuleEventAsServer();
                                }
                            }
                            else
                            {
                                Logging.Instance.Error("component was null in PatchMissionLobbyComponent_SendPeerInformationsToPeer!");
                            }
                        }
                        else
                        {
                            Logging.Instance.Error("peer was null in PatchMissionLobbyComponent_SendPeerInformationsToPeer!");
                        }
                    }
                    else
                    {
                        Debug.Print(">#< Can't send the info of " + disconnectedPeer.UserName + " to " + peer.UserName + ".", color: Debug.DebugColor.BrightWhite, debugFilter: 17179869184UL);
                        Debug.Print(string.Format("isDisconnectedPeer: {0}", (object)flag), color: Debug.DebugColor.BrightWhite, debugFilter: 17179869184UL);
                        Debug.Print(string.Format("networkPeer.IsSynchronized: {0}", (object)disconnectedPeer.IsSynchronized), color: Debug.DebugColor.BrightWhite, debugFilter: 17179869184UL);
                        Debug.Print(string.Format("peer == networkPeer: {0}", (object)(peer == disconnectedPeer)), color: Debug.DebugColor.BrightWhite, debugFilter: 17179869184UL);
                        Debug.Print(string.Format("networkPeer.JustReconnecting: {0}", (object)disconnectedPeer.JustReconnecting), color: Debug.DebugColor.BrightWhite, debugFilter: 17179869184UL);
                    }
                }
                else
                {
                    Logging.Instance.Error("disconnectedPeer was null in PatchMissionLobbyComponent_SendPeerInformationsToPeer!");
                }
            }
            return false;
        }

    }

    class PatchMissionLobbyComponent_OnPlayerKills
    {
        static bool hitOnce = false;

        public static bool Prefix(MissionLobbyComponent __instance, MissionPeer killerPeer, Agent killedAgent, MissionPeer assistorPeer)
        {
            if (!hitOnce)
            {
                Logging.Instance.Info("PatchMissionLobbyComponent_OnPlayerKills.Prefix has been hit once");
                hitOnce = true;
            }

            if (killedAgent.MissionPeer == null)
            {
                NetworkCommunicator networkCommunicator = GameNetwork.NetworkPeers.SingleOrDefault((NetworkCommunicator x) => x.GetComponent<MissionPeer>() != null && x.GetComponent<MissionPeer>().ControlledFormation != null && x.GetComponent<MissionPeer>().ControlledFormation == killedAgent.Formation);
                if (networkCommunicator != null)
                {
                    MissionPeer component = networkCommunicator.GetComponent<MissionPeer>();
                    killerPeer.OnKillAnotherPeer(component);
                }
            }
            else
            {
                killerPeer.OnKillAnotherPeer(killedAgent.MissionPeer);
            }
            MissionMultiplayerGameModeBase gameMode = Traverse.Create(__instance).Field("_gameMode").GetValue() as MissionMultiplayerGameModeBase;
            if (gameMode != null)
            {
                if(killerPeer.ControlledAgent != null)
                {
                    TaleWorlds.MountAndBlade.Team killerTeam = killerPeer.Team;
                    if(killerPeer.ControlledAgent != null)
                    {
                        killerTeam = killerPeer.ControlledAgent.Team;
                    }
                    if(killerTeam != null)
                    {
                        if (killerTeam.IsEnemyOf(killedAgent.Team))
                        {
                            Traverse.Create(killerPeer).Field("Score").SetValue(killerPeer.Score + gameMode.GetScoreForKill(killedAgent));
                            Traverse.Create(killerPeer).Field("KillCount").SetValue(killerPeer.KillCount + 1);
                        }
                        else
                        {
                            Traverse.Create(killerPeer).Field("Score").SetValue((int)((float)gameMode.GetScoreForKill(killedAgent) * 1.5f));
                            Traverse.Create(killerPeer).Field("KillCount").SetValue(killerPeer.KillCount - 1);
                        }
                    }
                    else
                    {
                        Logging.Instance.Error("Both killerPeer.Team and killerPeer.ControlledAgent.Team were null for MissionLobbyComponent patch");
                    }
                    
                }
                else
                {
                        Logging.Instance.Error("killerPeer.ControlledAgent was null for MissionLobbyComponent patch");
                }
                
            }
            else
            {
                Logging.Instance.Error("gameMode was null for MissionLobbyComponent patch");
            }
            MissionScoreboardComponent missionScoreboardComponent = Traverse.Create(__instance).Field("_missionScoreboardComponent").GetValue() as MissionScoreboardComponent;
            
            if (missionScoreboardComponent != null)
            {
                missionScoreboardComponent.PlayerPropertiesChanged(killerPeer.GetNetworkPeer());
            }
            else
            {
                Logging.Instance.Error("missionScoreboardComponent was null for MissionLobbyComponent patch");
            }

            GameNetwork.BeginBroadcastModuleEvent();
            GameNetwork.WriteMessage(new KillDeathCountChange(killerPeer.GetNetworkPeer(), null, killerPeer.KillCount, killerPeer.AssistCount, killerPeer.DeathCount, killerPeer.Score));
            GameNetwork.EndBroadcastModuleEvent(GameNetwork.EventBroadcastFlags.None, null);
            return false;
        }
    }
        
    
}
