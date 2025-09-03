using RabbitMQ.Client;
using Microsoft.Data.Sqlite;
using DeviceBase;

namespace DeviceSim
{
    public struct Mouse2
    {
        double voltage;
        double resistance;
    }

    public struct Config_Mouse2
    {
        string name;
        double alarmThreshold;
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            DeviceType deviceType;
            string? deviceTypeString = Environment.GetEnvironmentVariable("DEVICE_TYPE");
            if (deviceTypeString is null) throw new Exception("Environment variable DEVICE_TYPE not specified");
            else
            {
                if (deviceTypeString == "MOUSE2") deviceType = DeviceType.MOUSE2;
                else if (deviceTypeString == "MOUSE2B") deviceType = DeviceType.MOUSE2B;
                else if (deviceTypeString == "MOUSECOMBO") deviceType = DeviceType.MOUSECOMBO;
                else if (deviceTypeString == "MAS2") deviceType = DeviceType.MAS2;
                else throw new Exception($"Unknown device type: {deviceTypeString}");
            }
            int dataFrequency;
            if (Environment.GetEnvironmentVariable("DATA_FREQUENCY") is not null)
                dataFrequency = int.Parse(Environment.GetEnvironmentVariable("DATA_FREQUENCY"));
            else
                dataFrequency = 5000;

            Device device = new Device(deviceType);

            ConnectionManager connection = new ConnectionManager(device);
            Task task = Task.Run(() => connection.StartTransmitting(dataFrequency));
            task.Wait();
        }
    }
}
