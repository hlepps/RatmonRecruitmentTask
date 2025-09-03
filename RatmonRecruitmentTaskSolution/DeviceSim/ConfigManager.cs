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
        string configFileName = null;
        public ConfigManager(Device device)
        {
            configFileName = Environment.GetEnvironmentVariable("CONFIG_FILENAME") + ".json";
            if (configFileName is null) throw new Exception("Environment variable CONFIG_FILENAME not specified");

            if (!File.Exists(configFileName))
            {
                device.Config.Name = device.deviceType.ToString();
                device.Config.UniqueId = Guid.CreateVersion7().ToString();
                var config = JsonSerializer.Serialize<Config>(device.Config);
                File.WriteAllText(configFileName, config);
            }
            else
            {
                string json = File.ReadAllText(configFileName);
                try
                {
                    switch (device.deviceType)
                    {
                        case DeviceBase.DeviceType.MOUSE2:
                            device.Config = JsonSerializer.Deserialize<Config_MOUSE2>(json);
                            break;
                        case DeviceBase.DeviceType.MOUSE2B:
                            device.Config = JsonSerializer.Deserialize<Config_MOUSE2B>(json);
                            break;
                        case DeviceBase.DeviceType.MOUSECOMBO:
                            device.Config = JsonSerializer.Deserialize<Config_MOUSECOMBO>(json);
                            break;
                        case DeviceBase.DeviceType.MAS2:
                            device.Config = JsonSerializer.Deserialize<Config_MAS2>(json);
                            break;
                    }
                }
                catch (Exception e)
                {
                    device.Config.Name = device.deviceType.ToString();
                    device.Config.UniqueId = Guid.CreateVersion7().ToString();
                    var config = JsonSerializer.Serialize<Config>(device.Config);
                    File.WriteAllText(configFileName, config);
                }
                if (device.Config is null) throw new Exception("Unable to deserialize config.json");
                Console.WriteLine($"ID:{device.Config.UniqueId}");
            }
        }
    }
}
