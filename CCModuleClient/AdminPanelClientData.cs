using CCModuleNetworkMessages.FromServer;
using ClientServerShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCModuleClient
{
    class AdminPanelClientData : AdminPanelData
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
    }
}
