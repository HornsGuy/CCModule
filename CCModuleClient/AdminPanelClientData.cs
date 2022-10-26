using CCModuleNetworkMessages.FromServer;
using System;
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

        public void Update(SyncAdminPanelMessage message)
        {
            UpdateTroopCapsIfDifferent(message.InfantryCap, message.RangedCap, message.CavalryCap, message.HorseArcherCap);
            UpdateAvailableMaps(message.AvailableMaps);
        }

        public bool UpdateTroopCapsIfDifferent(int infCap, int rangeCap, int cavCap, int haCap)
        {
            if (InfantryCap != infCap || RangedCap != rangeCap || CavalryCap != cavCap || HorseArcherCap != haCap)
            {
                InfantryCap = infCap;
                RangedCap = rangeCap;
                CavalryCap = cavCap;
                HorseArcherCap = haCap;

                TroopCapBehavior.UpdateTroopCaps(InfantryCap, RangedCap, CavalryCap, HorseArcherCap);

                return true;
            }
            return false;
        }

        public void UpdateAvailableMaps(List<string> maps)
        {
            AvailableMaps = maps;
            AdminPanelMissionView.UpdateAvailableMaps(maps);
        }

        public int InfantryCap { get; private set; }
        public int RangedCap { get; private set; }
        public int CavalryCap { get; private set; }
        public int HorseArcherCap { get; private set; }

        public List<string> AvailableMaps { get; private set; } = new List<string>();
    }
}
