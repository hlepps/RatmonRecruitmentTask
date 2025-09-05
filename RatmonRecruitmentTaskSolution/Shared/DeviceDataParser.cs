using System.Runtime.InteropServices;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shared
{
    public class DeviceDataParser
    {
        /// <summary>
        /// Parses JSON with device data and returns IDeviceData object with correct type, and outs DeviceType
        /// </summary>
        /// <param name="data"></param>
        /// <param name="deviceType"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DeviceDataBase ParseJSON(string data, out DeviceType deviceType)
        {
            DeviceDataBase parsedData;
            if (data.Contains("Voltage") && data.Contains("Resistance") && data.Contains("Reflectograms"))
            {
                parsedData = JsonSerializer.Deserialize<Shared.DeviceData_MOUSECOMBO>(data);
                deviceType = DeviceType.MOUSECOMBO;
            }
            else if (data.Contains("Voltage") && data.Contains("Resistance") && data.Contains("LeakLocation"))
            {
                parsedData = JsonSerializer.Deserialize<Shared.DeviceData_MOUSE2B>(data);
                deviceType = DeviceType.MOUSE2B;
            }
            else if (data.Contains("Voltage") && data.Contains("Resistance"))
            {
                parsedData = JsonSerializer.Deserialize<Shared.DeviceData_MOUSE2>(data);
                deviceType = DeviceType.MOUSE2;
            }
            else if (data.Contains("Temperature") && data.Contains("Humidity"))
            {
                parsedData = JsonSerializer.Deserialize<Shared.DeviceData_MAS2>(data);
                deviceType = DeviceType.MAS2;
            }
            else throw new Exception("Incorrect data - device type cannot be parsed");

            return parsedData;
        }

        /// <summary>
        /// Parses JSON with device data and returns IDeviceData object with correct type
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DeviceDataBase ParseJSON(string data)
        {
            return ParseJSON(data, out DeviceType deviceType);
        }
    }
}
