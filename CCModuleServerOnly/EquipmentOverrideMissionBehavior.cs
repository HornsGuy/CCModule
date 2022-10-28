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

namespace CCModuleServerOnly
{

    class EquipmentData
    {
        public string name { get; set; } = "";
        public string ID { get; set; } = "";
        public List<Tuple<string, string>> equipmentToOverride { get; set; } = new List<Tuple<string, string>>();
    }

    class EquipmentOverrideMissionBehavior : MissionLogic
    {
        private string jsonPath = "equipment.json";

        Dictionary<string, List<Tuple<EquipmentIndex, string>>> equipmentToOverride = new Dictionary<string, List<Tuple<EquipmentIndex, string>>>();

        // Prevents infinite looping because we are giving equipment by spawning a new agent
        Dictionary<string, bool> lastSpawnForPlayerWasOverride = new Dictionary<string, bool>();

        public EquipmentOverrideMissionBehavior()
        {
            if (File.Exists(jsonPath))
            {
                Debug.Print("Loadding equipment.json", 0, Debug.DebugColor.Yellow);
                List<EquipmentData> equipmentOverrides = JsonConvert.DeserializeObject<List<EquipmentData>>(File.ReadAllText(jsonPath));

                foreach (var ed in equipmentOverrides)
                {
                    Debug.Print("Override found for " + ed.name, 0, Debug.DebugColor.Yellow);
                    if (ed.ID != "ID Goes Here")
                    {
                        equipmentToOverride.Add(ed.ID, ConvertEquipmentFromFile(ed.equipmentToOverride));
                        lastSpawnForPlayerWasOverride[ed.ID] = false;
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

        public override MissionBehaviorType BehaviorType
        {
            get
            {
                return MissionBehaviorType.Other;
            }
        }

        private bool PlayerHasEquipmentToBeOverridden(string ID)
        {
            return equipmentToOverride.ContainsKey(ID);
        }

        private bool AgentIsReadyToBeReplaced(Agent a)
        {
            if(a != null)
            {
                return a.HasBeenBuilt &&
                    a.State == AgentState.Active &&
                    a.IsActive() &&
                    a.IsPlayerControlled &&
                    a.MissionRepresentative != null &&
                    a.MissionPeer != null;
            }
            return false;
        }

        private void ThreadProc(Object obj)
        {
            int count = 0;

            Tuple<string, List<Tuple<EquipmentIndex, string>>> overrideInfo = (Tuple<string, List<Tuple<EquipmentIndex, string>>>)obj;

            while (!AgentIsReadyToBeReplaced(AdminPanel.Instance.GetPlayerNetworkPeerFromID(overrideInfo.Item1).ControlledAgent))
            {
                Thread.Sleep(1000);
                count += 1;
            }

            Thread.Sleep(1000);

            Debug.Print($"Slept for {count} iterations");

            AdminPanel.Instance.GivePlayerAgentCosmeticEquipment(overrideInfo.Item1, overrideInfo.Item2);
        }

        public override void OnAgentBuild(Agent agent, Banner banner)
        {

            if (agent.IsHuman)
            {
                // Get the palyer's ID and check if they have equipment
                string ID = agent.MissionPeer.Peer.Id.ToString();

                //Debug.Print("Player Spawning", 0, Debug.DebugColor.Yellow);

                if (PlayerHasEquipmentToBeOverridden(ID))
                {
                    //Debug.Print("Player Has Overrides", 0, Debug.DebugColor.Yellow);
                    if (!lastSpawnForPlayerWasOverride[ID])
                    {
                        //Debug.Print("Overriding " + ID, 0, Debug.DebugColor.Yellow); 
                        lastSpawnForPlayerWasOverride[ID] = true;

                        Thread t = new Thread(new ParameterizedThreadStart(ThreadProc));
                        t.Start(new Tuple<string, List<Tuple<EquipmentIndex, string>>>(ID, equipmentToOverride[ID]));
                    }
                    else
                    {
                        //Debug.Print("Skipping Override " + ID, 0, Debug.DebugColor.Yellow);
                        lastSpawnForPlayerWasOverride[ID] = false;
                    }

                }
            }
        }
    }
}
