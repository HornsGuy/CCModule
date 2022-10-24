﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCModuleClient
{
    class AdminPanelClientData
    {
        static AdminPanelClientData _instance;
        static public AdminPanelClientData Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new AdminPanelClientData();
                }
                return _instance;
            }
        }

        public bool UpdateTroopCapsIfDifferent(int infCap, int rangeCap, int cavCap, int haCap)
        {
            if (InfantryCap != infCap || RangedCap != rangeCap || CavalryCap != cavCap || HorseArcherCap != haCap)
            {
                InfantryCap = infCap;
                RangedCap = rangeCap;
                CavalryCap = cavCap;
                HorseArcherCap = haCap;
                return true;
            }
            return false;
        }

        public int InfantryCap { get; set; }
        public int RangedCap { get; set; }
        public int CavalryCap { get; set; }
        public int HorseArcherCap { get; set; }
    }
}
