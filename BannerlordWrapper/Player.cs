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
        public Troop Troop { get; private set; }
        public PlayerCosmetics PlayerCosmetics { get; private set; }

        public Player(string iD, string name, TeamType teamType)
        {
            ID = iD;
            Name = name;
            Team = TeamWrapper.Instance.GetTeamFromType(teamType);
        }

        public Player(string iD, string name, TeamType teamType, int troopIndex)
        {
            ID = iD;
            Name = name;
            Team = TeamWrapper.Instance.GetTeamFromType(teamType);
            ChangeSelectedTroopIndex(troopIndex);
        }

        public void ChangeTeam(TeamType teamType)
        {
            Team = TeamWrapper.Instance.GetTeamFromType(teamType);
            Logging.Instance.Debug($"{this} joined team {teamType}");
        }

        public void ChangeSelectedTroopIndex(int newTroop)
        {
            if(!Team.IsSpectator())
            {
                if (Team.HasTroop(newTroop))
                {
                    Logging.Instance.Debug($"Changing {this} troop to {newTroop}");
                    Troop = Team.GetTroop(newTroop);
                }
                else
                {
                    Logging.Instance.Error($"Index {newTroop} does not exist for Team {Team}. Defaulting to 0");
                    Troop = Team.GetTroop(0);
                }
            }
        }

        public void SetPlayerCosmetics(PlayerCosmetics cosmetics)
        {
            PlayerCosmetics = cosmetics;
        }

        public void ClearPlayerCosmetics()
        {
            PlayerCosmetics = null;
        }

        public bool PlayerHasCosmetics()
        {
            return PlayerCosmetics != null;
        }

        public bool PlayerHasLordArmor()
        {
            return PlayerCosmetics != null && PlayerCosmetics.IsLordArmor();
        }

        public override string ToString()
        {
            return $"{Name}:{ID}";
        }
    }
}
