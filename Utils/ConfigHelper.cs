using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterNameplates.Utils
{
    public class Config
    {
        public bool nameplatesEnabled { get; set; }
        public bool allowToggle { get; set; }
        public bool adminOverride { get; set; }
    }

    public class ConfigHelper
    {
        public static void EnsureConfig(string path)
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("No config.json");

                JObject nameplatesConfig = new JObject();
                nameplatesConfig.Add("nameplatesEnabled", true);
                nameplatesConfig.Add("allowToggle", true);
                nameplatesConfig.Add("adminOverride", true);

                using (StreamWriter file = File.CreateText(path))
                using (JsonTextWriter writer = new JsonTextWriter(file))
                {
                    nameplatesConfig.WriteTo(writer);
                    Console.WriteLine("Generated BetterNameplates config");
                }
            }
        }

        public static Config ReadConfig(string path)
        {
            using (StreamReader file = File.OpenText(path))
            using (JsonTextReader reader = new JsonTextReader(file))
            {
                return JsonConvert.DeserializeObject<Config>(JToken.ReadFrom(reader).ToString());
            }
        }
    }
}
