using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CCModuleServerOnly
{
    class Player
    {
        public Player(string name, string ID)
        {
            this.name = name;
            this.ID = ID;
        }
        public string name { get; set; }
        public string ID { get; set; }
    }

    public class PlayerManager
    { 
        private static PlayerManager _instance;
        public static PlayerManager Instance
        {
            get 
            {
                if(_instance == null)
                {
                    _instance =  new PlayerManager();
                }
                return _instance;
            }
        }

        private const string exampleID = "IDGoesHere";

        private string adminFilePath;
        private string banFilePath;

        List<Player> bannedPlayers = new List<Player>();
        HashSet<string> bannedIds = new HashSet<string>();

        List<Player> admins = new List<Player>();
        HashSet<string> adminIds = new HashSet<string>();

        PlayerManager()
        {
            string basePath = AppDomain.CurrentDomain.BaseDirectory;
            adminFilePath = Path.Combine(basePath, "admins.json");
            banFilePath = Path.Combine(basePath, "bans.json");

            LoadSavedPlayerData(adminFilePath, ref adminIds);
            LoadSavedPlayerData(banFilePath, ref bannedIds);
        }

        public void Setup()
        {

        }

        private void LoadSavedPlayerData(string filePath, ref HashSet<string> toFill)
        {
            if (File.Exists(filePath))
            { 
                List<Player> adminList = JsonConvert.DeserializeObject<List<Player>>(File.ReadAllText(filePath));
                foreach (var admin in adminList)
                {
                    if(admin.ID != exampleID)
                    {
                        toFill.Add(admin.ID);
                    }
                }
            }
            else
            {
                string firstContents = JsonConvert.SerializeObject(new List<Player>() { new Player("NameGoesHere", exampleID) },Formatting.Indented);
                File.WriteAllText(filePath, firstContents);
            }
        }

        private void AddPlayerToList(string name, string ID, string fileName, ref List<Player> list)
        {
            list.Add(new Player(name,ID));
            File.WriteAllText(fileName, JsonConvert.SerializeObject(list));
        }

        private void RemovePlayerFromList(string ID, string fileName, ref List<Player> list)
        {
            foreach (var player in list)
            {
                if(player.ID == ID)
                {
                    list.Remove(player);
                    break;
                }
            }

            File.WriteAllText(fileName, JsonConvert.SerializeObject(list));
        }

        public void BanPlayer(string name, string ID)
        {
            bannedIds.Add(ID);
            AddPlayerToList(name, ID, banFilePath, ref bannedPlayers);
        }

        public void UnbanPlayer(string ID)
        {
            bannedIds.Remove(ID);
            RemovePlayerFromList(ID, banFilePath, ref bannedPlayers);
        }

        public void AddAdmin(string name, string ID)
        {
            adminIds.Add(ID);
            AddPlayerToList(name, ID, adminFilePath, ref admins);
        }

        public bool PlayerIsBanned(string ID)
        {
            return bannedIds.Contains(ID);
        }

        public bool PlayerIsAdmin(string ID)
        {
            return adminIds.Contains(ID);
        }

        public void Reset()
        {
            _instance = null;
        }
    }
}
