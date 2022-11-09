using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BannerlordWrapper;
using TaleWorlds.MountAndBlade;
using System.Xml;

namespace CCModuleServerOnly
{
    public class MapLoader
    {

        static bool IgnoreLine(string line)
        {
            return line.StartsWith("#") || line == "";
        }

        public static void LoadMaps( string gameTypesFile, string mapCSVPath)
        {
            AdminPanel.Instance.InitializeGameTypesForMaps(gameTypesFile);

            string[] lines = File.ReadAllLines(mapCSVPath);
            if(lines.Length != 0)
            {
                foreach (var line in lines)
                {
                    bool loaded = false;
                    if(!IgnoreLine(line) && line.Contains(","))
                    {
                        List<string> splitLine = new List<string>(line.Split(','));
                        if(splitLine.Count >= 2)
                        {
                            string mapName = splitLine[0];

                            foreach (string gameType in splitLine.GetRange(1, splitLine.Count - 1))
                            {
                                AdminPanel.Instance.AddMap(gameType, mapName);
                            }

                            loaded = true;
                        }
                    }

                    if(!loaded && !IgnoreLine(line))
                    {
                        Logging.Instance.Warn($"Unable to load the following line from {mapCSVPath}: {line}");
                    }
                }
            }
        }
    }
}
