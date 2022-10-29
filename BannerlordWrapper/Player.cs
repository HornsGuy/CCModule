using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static BannerlordWrapper.TeamWrapper;

namespace BannerlordWrapper
{
    public class Player
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public Team Team { get; private set; }
        public int SelectedTroopIndex { get; private set; }


        public Player(string iD, string name, TeamType teamType, int selectedTroopIndex=0)
        {
            ID = iD;
            Name = name;
            Team = TeamWrapper.Instance.GetTeamFromType(teamType);
            SelectedTroopIndex = selectedTroopIndex;
        }

        public void ChangeTeam(TeamType teamType)
        {
            Team = TeamWrapper.Instance.GetTeamFromType(teamType);
            Logging.Instance.Debug($"{this} joined team {teamType}");
        }

        public void ChangeSelectedTroopIndex(int newTroop)
        {
            Logging.Instance.Debug($"{this} troop to {newTroop}:{Team.GetTroopName(newTroop)}");
            SelectedTroopIndex = newTroop;
        }

        public override string ToString()
        {
            return $"{Name}:{ID}";
        }
    }
}
