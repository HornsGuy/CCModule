using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerShared
{
    public static class TroopCapLogic
    {
        public static Dictionary<string, float> GetCurrentTeamClassTypeBreakdown(List<int> myTeamTroopIndexes, Dictionary<int, string> troopIndexToTroopType, List<string> classTypes)
        {

            // Add the default 3 we care about
            Dictionary<string, float> troopTypeCount = new Dictionary<string, float>();

            foreach(var classType in classTypes)
            {
                troopTypeCount.Add(classType, 0);
            }

            float total = 0;
            foreach (var troopIndex in myTeamTroopIndexes)
            {
                troopTypeCount[troopIndexToTroopType[troopIndex]] += 1;
                total += 1;
            }

            // Take the totals and convert it into the breakdown
            Dictionary<string, float> toReturn = new Dictionary<string, float>();

            foreach (var keyVal in troopTypeCount)
            {
                toReturn.Add(keyVal.Key, (keyVal.Value / total) * 100.0f);
            }

            return toReturn;
        }

        public static Dictionary<string, bool> GetTroopClassAvailabilityDictionary(Dictionary<string, float> currentTroopBreakdown, Dictionary<string, int> troopCapPercent)
        {
            Dictionary<string, bool> toReturn = new Dictionary<string, bool>();

            foreach (var keyVal in currentTroopBreakdown)
            {
                toReturn.Add(keyVal.Key, keyVal.Value < troopCapPercent[keyVal.Key]);
            }

            return toReturn;
        }
    }
}
