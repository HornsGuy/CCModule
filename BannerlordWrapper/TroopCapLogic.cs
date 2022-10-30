using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerlordWrapper
{

    public static class TroopCapLogic
    {

        public static bool TroopUnderCapForTeam(Team team, int troopIndex, int troopCapForPlayersTroopType)
        {
            if (troopCapForPlayersTroopType == 0 || team.TeamType == TeamType.Spectator)
            {
                return false;
            }
            else if (troopCapForPlayersTroopType == 100)
            {
                return true;
            }

            Dictionary<TroopType, double> currentTroopBreakdown = PlayerWrapper.Instance.GetTroopTypeBreakdownForTeam(team);

            return currentTroopBreakdown[team.GetTroopType(troopIndex)] <= troopCapForPlayersTroopType;
        }
    }
}
