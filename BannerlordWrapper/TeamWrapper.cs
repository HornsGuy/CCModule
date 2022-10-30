using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerlordWrapper
{
    public enum TeamType
    {
        Spectator = -1,
        Attacker = 0,
        Defender = 1
    }

    public class TeamWrapper
    {
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
            Reset();
        }

        public void Reset()
        {
            _teams.Clear();
            _teams.Add(TeamType.Spectator, new Team(TeamType.Spectator,"Spectator"));
            _teams.Add(TeamType.Attacker, new Team(TeamType.Attacker,"Undefined"));
            _teams.Add(TeamType.Defender, new Team(TeamType.Defender,"Undefined"));
        }

        public Team GetTeamFromType(TeamType type)
        {
            return _teams[type];
        }

        public void SetFactionForTeam(TeamType type, string faction, Dictionary<int,Troop> indexToTroop)
        {
            Logging.Instance.Debug($"{type} faction set to {faction}");
            _teams[type].ChangeFaction(faction, indexToTroop);
        }

    }
}
