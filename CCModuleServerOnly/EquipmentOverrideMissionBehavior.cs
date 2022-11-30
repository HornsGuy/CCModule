using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TaleWorlds.MountAndBlade;
using TaleWorlds.Core;
using Newtonsoft.Json;
using TaleWorlds.Library;
using System.Threading;
using TaleWorlds.ObjectSystem;
using BannerlordWrapper;

namespace CCModuleServerOnly
{

    class EquipmentData
    {
        public string name { get; set; } = "";
        public string ID { get; set; } = "";
        public List<Tuple<string, string>> equipmentToOverride { get; set; } = new List<Tuple<string, string>>();
    }

    class EquipmentOverride
    {

        static EquipmentOverride _instance;
        public static EquipmentOverride Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new EquipmentOverride();
                }
                return _instance;
            }
        }

        private string jsonPath = "equipment.json";

        Dictionary<string, List<Tuple<EquipmentIndex, string>>> equipmentToOverride = new Dictionary<string, List<Tuple<EquipmentIndex, string>>>();

        public EquipmentOverride()
        {
        }

        public void Setup()
        {
            PlayerWrapper.Instance.ClearAllCosmetics();
            equipmentToOverride.Clear();

            if (File.Exists(jsonPath))
            {
                Debug.Print("Loadding equipment.json", 0, Debug.DebugColor.Yellow);
                List<EquipmentData> equipmentOverrides = JsonConvert.DeserializeObject<List<EquipmentData>>(File.ReadAllText(jsonPath));

                foreach (var ed in equipmentOverrides)
                {
                    Debug.Print("Override found for " + ed.name, 0, Debug.DebugColor.Yellow);
                    if (ed.ID != "Player ID Goes Here")
                    {
                        equipmentToOverride.Add(ed.ID, ConvertEquipmentFromFile(ed.equipmentToOverride));
                    }
                }
            }
            else
            {
                List<EquipmentData> toSerialize = new List<EquipmentData>();

                toSerialize.Add(generateExampleData());
                toSerialize.Add(generateExampleData());
                toSerialize.Add(generateExampleData());

                string jsonContents = JsonConvert.SerializeObject(toSerialize, Formatting.Indented);
                File.WriteAllText(jsonPath, jsonContents);
            }
        }

        private EquipmentData generateExampleData()
        {
            EquipmentData ed = new EquipmentData();
            ed.name = "Player Name Goes Here";
            ed.ID = "Player ID Goes Here";
            ed.equipmentToOverride.Add(new Tuple<string, string>("Head", "item_id"));
            ed.equipmentToOverride.Add(new Tuple<string, string>("Shoulders", "item_id"));
            ed.equipmentToOverride.Add(new Tuple<string, string>("Body", "item_id"));
            ed.equipmentToOverride.Add(new Tuple<string, string>("Gloves", "item_id"));
            ed.equipmentToOverride.Add(new Tuple<string, string>("Legs", "item_id"));

            return ed;
        }

        static Dictionary<string, EquipmentIndex> stringToEquipmentIndex = new Dictionary<string, EquipmentIndex> {
            { "Head", EquipmentIndex.Head },
            { "Shoulders", EquipmentIndex.Cape },
            { "Body", EquipmentIndex.Body },
            { "Gloves", EquipmentIndex.Gloves },
            { "Legs", EquipmentIndex.Leg }
        };

        private List<Tuple<EquipmentIndex, string>> ConvertEquipmentFromFile(List<Tuple<string, string>> toConvert)
        {
            List<Tuple<EquipmentIndex, string>> toReturn = new List<Tuple<EquipmentIndex, string>>();

            foreach (var tuple in toConvert)
            {
                if (stringToEquipmentIndex.ContainsKey(tuple.Item1))
                {
                    toReturn.Add(new Tuple<EquipmentIndex, string>(stringToEquipmentIndex[tuple.Item1], tuple.Item2));
                }
            }

            return toReturn;
        }

        public PlayerCosmetics GetLoadedCosmeticsIfTheyExist(string ID)
        {
            if(equipmentToOverride.ContainsKey(ID))
            {
                string head = "";
                string shoulder = "";
                string body = "";
                string hands = "";
                string legs = "";
                foreach (var overrideItem in equipmentToOverride[ID])
                {
                    switch (overrideItem.Item1)
                    {
                        case EquipmentIndex.Head:
                            head = overrideItem.Item2;
                            break;
                        case EquipmentIndex.Body:
                            body = overrideItem.Item2;
                            break;
                        case EquipmentIndex.Leg:
                            legs = overrideItem.Item2;
                            break;
                        case EquipmentIndex.Gloves:
                            hands = overrideItem.Item2;
                            break;
                        case EquipmentIndex.Cape:
                            shoulder = overrideItem.Item2;
                            break;
                    }
                }
                return new PlayerCosmetics(head, shoulder, body, hands, legs, true);
            }
            return null;
        }

        private List<Tuple<EquipmentIndex, string>> PlayerCosmeticsToTuples(PlayerCosmetics playerCosmetics)
        {
            List<Tuple<EquipmentIndex, string>> toReturn = new List<Tuple<EquipmentIndex, string>>();

            toReturn.Add(new Tuple<EquipmentIndex,string>(EquipmentIndex.Head, playerCosmetics.HeadItem));
            toReturn.Add(new Tuple<EquipmentIndex,string>(EquipmentIndex.Cape, playerCosmetics.ShoulderItem));
            toReturn.Add(new Tuple<EquipmentIndex,string>(EquipmentIndex.Body, playerCosmetics.BodyItem));
            toReturn.Add(new Tuple<EquipmentIndex,string>(EquipmentIndex.Gloves, playerCosmetics.HandItem));
            toReturn.Add(new Tuple<EquipmentIndex,string>(EquipmentIndex.Leg, playerCosmetics.LegItem));

            return toReturn;
        }

        public Equipment GetOverriddenEquipment(string ID, Equipment originalEquipment)
        {

            Equipment newEquipment = originalEquipment;

            PlayerCosmetics cosmetics = PlayerWrapper.Instance.GetPlayer(ID).PlayerCosmetics;

            foreach (var itemToOverride in PlayerCosmeticsToTuples(cosmetics))
            {
                ItemObject item = MBObjectManager.Instance.GetObject<ItemObject>(itemToOverride.Item2);
                if (item != null)
                {
                    EquipmentElement equipmentElement = originalEquipment[itemToOverride.Item1];
                    equipmentElement.CosmeticItem = item;
                    newEquipment[itemToOverride.Item1] = equipmentElement;
                }
            }

            return newEquipment;
        }

    }
}
