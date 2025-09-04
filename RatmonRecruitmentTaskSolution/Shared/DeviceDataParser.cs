using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shared
{
    public class DeviceDataParser
    {
        public static IDeviceData ParseJSON(string data)
        {
            IDeviceData parsedData;
            if (data.Contains("Voltage") && data.Contains("Resistance") && data.Contains("Reflectograms"))
                parsedData = JsonSerializer.Deserialize<Shared.DeviceData_MOUSECOMBO>(data);
            else if (data.Contains("Voltage") && data.Contains("Resistance") && data.Contains("LeakLocation"))
                parsedData = JsonSerializer.Deserialize<Shared.DeviceData_MOUSE2B>(data);
            else if (data.Contains("Voltage") && data.Contains("Resistance"))
                parsedData = JsonSerializer.Deserialize<Shared.DeviceData_MOUSE2>(data);
            else if (data.Contains("Temperature") && data.Contains("Humidity"))
                parsedData = JsonSerializer.Deserialize<Shared.DeviceData_MOUSECOMBO>(data);
            else throw new Exception("Incorrect data - device type cannot be parsed");

            return parsedData;
        }
    }
}
