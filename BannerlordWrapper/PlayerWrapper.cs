using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BannerlordWrapper.TeamWrapper;

namespace BannerlordWrapper
{
    public class PlayerWrapper
    {
        static PlayerWrapper _instance;
        public static PlayerWrapper Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new PlayerWrapper();
                }
                return _instance;
            }
        }

        Dictionary<string, Player> _players = new Dictionary<string, Player>();
        

        public PlayerWrapper()
        {

        }

        public void Reset()
        {
            _players.Clear();
        }

        public bool AddPlayer(Player p)
        {
            if(!_players.ContainsKey(p.ID))
            {
                _players.Add(p.ID, p);
                Logging.Instance.Info($"{p} has joined the server");
                return true;
            }
            return false;
        }

        public bool RemovePlayer(string ID)
        {
            if(_players.ContainsKey(ID))
            {
                Logging.Instance.Info($"{_players[ID]} has left the server");
                return _players.Remove(ID);
            }
            return false;
        }

        public void SetAllPlayersToSpectator()
        {
            foreach (var player in _players.Values)
            {
                player.ChangeTeam(TeamType.Spectator);
            }
        }

        public Player GetPlayer(string ID)
        {
            return _players[ID];
        }

        public void SetTroopIndexForPlayer(string ID, int index)
        {
            if(_players.ContainsKey(ID))
            {
                _players[ID].ChangeSelectedTroopIndex(index);
            }
        }

        public void SetPlayerTeam(string ID, TeamType teamType)
        {
            if (_players.ContainsKey(ID))
            {
                _players[ID].ChangeTeam(teamType);
            }
        }

        public List<int> GetTroopIndecesForTeam(TeamWrapper.TeamType team)
        {
            List<int> indeces = new List<int>();

            foreach (var player in _players.Values)
            {
                if(team == player.Team.TeamType)
                {
                    indeces.Add(player.SelectedTroopIndex);
                }
            }

            return indeces;
        }
    }
}
