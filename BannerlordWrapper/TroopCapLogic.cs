using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerlordWrapper
{

    public static class TroopCapLogic
    {
        public static bool PlayerCanSpawn(Player player, int troopCapForPlayersTroopType)
        {
            if(troopCapForPlayersTroopType == 0 || player.Team.TeamType == TeamType.Spectator)
            {
                return false;
            }
            else if(troopCapForPlayersTroopType == 100 )
            {
                return true;
            }

            Dictionary<TroopType, double> currentTroopBreakdown = PlayerWrapper.Instance.GetTroopTypeBreakdownForTeam(player.Team);

            return currentTroopBreakdown[player.Troop.TroopType] <= troopCapForPlayersTroopType;
        }
    }
}
