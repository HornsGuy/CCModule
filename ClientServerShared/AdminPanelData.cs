using BannerlordWrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientServerShared
{
    public class AdminPanelData
    {

        public int InfantryCap { get; private set; }
        public int RangedCap { get; private set; }
        public int CavalryCap { get; private set; }
        public int HorseArcherCap { get; private set; }

        public List<string> AvailableMaps { get; private set; } = new List<string>();

        public delegate void OnTroopCapsUpdatedEvent();
        public event OnTroopCapsUpdatedEvent OnTroopCapsUpdated;

        public delegate void OnAvailableMapsUpdatedEvent(List<string> maps);
        public event OnAvailableMapsUpdatedEvent OnAvailableMapsUpdated;

        public AdminPanelData()
        {
            InfantryCap = 100;
            RangedCap = 100;
            CavalryCap = 100;
            HorseArcherCap = 100;
        }

        public bool UpdateTroopCapsIfDifferent(int infCap, int rangeCap, int cavCap, int haCap)
        {
            if (InfantryCap != infCap || RangedCap != rangeCap || CavalryCap != cavCap || HorseArcherCap != haCap)
            {
                InfantryCap = infCap;
                RangedCap = rangeCap;
                CavalryCap = cavCap;
                HorseArcherCap = haCap;

                if(OnTroopCapsUpdated != null)
                {
                    OnTroopCapsUpdated();
                }

                return true;
            }
            return false;
        }

        public void UpdateAvailableMaps(List<string> maps)
        {
            AvailableMaps = maps;
            if(OnAvailableMapsUpdated != null)
            {
                OnAvailableMapsUpdated(maps);
            }
        }

        public bool TroopCapsAreInEffect()
        {
            return InfantryCap != 100 || RangedCap != 100 || CavalryCap != 100 || HorseArcherCap != 100;
        }

        public int GetTroopCapForTroopType(TroopType troopType)
        {
            if(troopType == TroopType.Infantry)
            {
                return InfantryCap;
            }
            else if (troopType == TroopType.Ranged)
            {
                return RangedCap;
            }
            else if (troopType == TroopType.Cavalry)
            {
                return CavalryCap;
            }
            else if (troopType == TroopType.HorseArcher)
            {
                return HorseArcherCap;
            }

            return 100; // If the troop is undefined, we just let them spawn
        }

    }
}
