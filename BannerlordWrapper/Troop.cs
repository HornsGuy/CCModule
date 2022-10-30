using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BannerlordWrapper
{
    public enum TroopType
    {
        NotFound = -1,
        Infantry = 0,
        Ranged,
        Cavalry,
        HorseArcher,
    }

    public class Troop
    {
        public int TroopIndex { get; private set; }
        public string Name { get; private set; }
        public TroopType TroopType { get; private set; }

        public Troop(int troopIndex, string name, TroopType troopType)
        {
            TroopIndex = troopIndex;
            Name = name;
            TroopType = troopType;
        }

        public override string ToString()
        {
            return $"{TroopIndex}:{Name}:{TroopType}";
        }

    }
}
