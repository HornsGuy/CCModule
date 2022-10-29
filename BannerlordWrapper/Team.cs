using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerlordWrapper
{

    public class Team
    {
        public TeamWrapper.TeamType TeamType { get; private set; }
        public string Faction { get; private set; }
        Dictionary<int, string> IndexToTroopName = new Dictionary<int, string>();

        public Team(string faction)
        {
            Faction = faction;
        }

        public void ChangeFaction(string faction, Dictionary<int,string> indexToTroopName)
        {
            Faction = faction;
            IndexToTroopName = indexToTroopName;
            LogFactionTroops();
        }

        private void LogFactionTroops()
        {
            foreach (var keyVal in IndexToTroopName)
            {
                Logging.Instance.Debug($"Added {keyVal.Key}:{keyVal.Value} to {Faction}");
            }
        }

        public string GetTroopName(int troopIndex)
        {
            string name = $"{troopIndex} Not Found";
            IndexToTroopName.TryGetValue(troopIndex, out name);
            return name;
        }

        public override string ToString()
        {
            return TeamType.ToString();
        }
    }
}
