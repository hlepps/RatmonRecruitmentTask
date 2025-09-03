using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeviceBase
{
    public class ConfigManager
    {
        public ConfigManager(Device device) {

            if(!File.Exists("config.json"))
            {
                var config = JsonSerializer.Serialize<Config>(device.Config);
                File.WriteAllText("config.json", config);
            }
        }
    }
}
