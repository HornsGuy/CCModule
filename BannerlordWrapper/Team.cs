using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerlordWrapper
{

    public class Team
    {
        public TeamType TeamType { get; private set; }
        public string Faction { get; private set; }
        Dictionary<int, Troop> IndexToTroop = new Dictionary<int, Troop>();

        public Team(TeamType teamType, string faction)
        {
            TeamType = teamType;
            Faction = faction;
        }

        public void ChangeFaction(string faction, Dictionary<int,Troop> indexToTroop)
        {
            Faction = faction;
            IndexToTroop = indexToTroop;
            LogFactionTroops();
        }

        private void LogFactionTroops()
        {
            foreach (var keyVal in IndexToTroop)
            {
                Logging.Instance.Debug($"Added {keyVal.Key}:{keyVal.Value.Name}:{keyVal.Value.TroopType} to {Faction}");
            }
        }

        public string GetTroopName(int troopIndex)
        {
            if(IndexToTroop.ContainsKey(troopIndex))
            {
                return IndexToTroop[troopIndex].Name;
            }
            return $"{troopIndex} Not Found";
        }

        public TroopType GetTroopType(int troopIndex)
        {
            if (IndexToTroop.ContainsKey(troopIndex))
            {
                return IndexToTroop[troopIndex].TroopType;
            }
            return TroopType.NotFound;
        }

        public Troop GetTroop(int troopIndex)
        {
            return IndexToTroop[troopIndex];
        }

        public bool HasTroop(int troopIndex)
        {
            return IndexToTroop.ContainsKey(troopIndex);
        }

        public override string ToString()
        {
            return $"{TeamType}:{Faction}";
        }
    }
}
