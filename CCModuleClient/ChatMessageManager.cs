using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade.GauntletUI;
using TaleWorlds.MountAndBlade.ViewModelCollection.Multiplayer;

namespace CCModuleClient
{
    public class ChatMessageManager
    {

        public static void AddMessage(string message, Color c)
        {
            Type typ = typeof(GauntletChatLogView);
            FieldInfo type = typ.GetField("_dataSource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            MPChatVM vm = (MPChatVM)type.GetValue(GauntletChatLogView.Current);

            if(vm != null)
            {
                MPChatLineVM mPChatLineVM = new MPChatLineVM(message, c, "Social");
                typeof(MPChatVM).GetMethod("AddChatLine", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(vm, new object[] { mPChatLineVM });
            }
        }

        public static void AddMessage(string message, float red, float green, float blue, float alpha = 1)
        {
            Type typ = typeof(GauntletChatLogView);
            FieldInfo type = typ.GetField("_dataSource", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            MPChatVM vm = (MPChatVM)type.GetValue(GauntletChatLogView.Current);

            if(vm != null)
            {
                MPChatLineVM mPChatLineVM = new MPChatLineVM(message, new Color(red / 255.0f, green / 255.0f, blue / 255.0f), "Social");
                typeof(MPChatVM).GetMethod("AddChatLine", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(vm, new object[] { mPChatLineVM });
            }
        }

        public static void ServerMessage(string message)
        {
            AddMessage(message, 50, 200, 50);
        }
    }
}
