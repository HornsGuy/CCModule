using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerlordWrapper
{
    public class TeamWrapper
    {
        public enum TeamType
        {
            Spectator = -1,
            Attacker = 0,
            Defender = 1
        }

        static TeamWrapper _instance;
        public static TeamWrapper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TeamWrapper();
                }
                return _instance;
            }
        }

        Dictionary<TeamType, Team> _teams = new Dictionary<TeamType, Team>();

        public TeamWrapper()
        {
            _teams.Add(TeamType.Spectator, new Team("Spectator"));
            _teams.Add(TeamType.Attacker, new Team("Undefined"));
            _teams.Add(TeamType.Defender, new Team("Undefined"));
        }

        public Team GetTeamFromType(TeamType type)
        {
            return _teams[type];
        }

        public void SetFactionForTeam(TeamType type, string faction, Dictionary<int,string> indexToName)
        {
            Logging.Instance.Debug($"{type} faction set to {faction}");
            _teams[type].ChangeFaction(faction, indexToName);
        }

        public Team GetTeamFromFaction(string faction)
        {
            foreach (var team in _teams.Values)
            {
                if(team.Faction == faction)
                {
                    return team;
                }
            }

            return null;
        }
    }
}
