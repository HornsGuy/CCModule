using CCModuleNetworkMessages.FromServer;
using ClientServerShared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CCModuleServerOnly
{
    public class AdminPanelServerData : AdminPanelData
    {
        static AdminPanelServerData _instance;
        public static AdminPanelServerData Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new AdminPanelServerData();
                }
                return _instance;
            }
        }

        public TroopCapServerMessage CreateTroopCapServerMessage(bool reportToUser)
        {
            return new TroopCapServerMessage(InfantryCap,RangedCap,CavalryCap,HorseArcherCap,reportToUser);
        }

        public SyncAdminPanelMessage CreateSyncMessage(bool printUpdatedValues)
        {
            SyncAdminPanelMessage syncMessage = new SyncAdminPanelMessage();
            syncMessage.SetTroopCaps(InfantryCap, RangedCap, CavalryCap, HorseArcherCap);
            syncMessage.AvailableMaps = AdminPanel.Instance.GetAllAvailableMaps();
            return syncMessage;
        }
    }
}
