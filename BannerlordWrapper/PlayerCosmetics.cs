using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BannerlordWrapper
{
    public class PlayerCosmetics
    {
        public string HeadItem { get; private set; } = "";
        public string ShoulderItem { get; private set; } = "";
        public string BodyItem { get; private set; } = "";
        public string HandItem { get; private set; } = "";
        public string LegItem { get; private set; } = "";

        bool lordArmor = false;

        public PlayerCosmetics(string head, string shoulder, string body, string hand, string leg, bool lordArmor = false)
        {
            HeadItem = head;
            ShoulderItem = shoulder;
            BodyItem = body;
            HandItem = hand;
            LegItem = leg;
            this.lordArmor = lordArmor;
        }

        public bool IsLordArmor()
        {
            return lordArmor;
        }

        public bool HasCosmetics()
        {
            return HeadItem != "" &&
                ShoulderItem != "" &&
                BodyItem != "" &&
                HandItem != "" &&
                LegItem != "";
        }

        public override string ToString()
        {
            return $"{HeadItem}:{ShoulderItem}:{BodyItem}:{HandItem}:{LegItem}";
        }

    }
}
