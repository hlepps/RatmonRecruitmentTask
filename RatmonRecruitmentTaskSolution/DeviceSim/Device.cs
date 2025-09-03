using MathNet.Numerics.Distributions;
using Shared;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeviceBase
{
    public enum DeviceType
    {
        MOUSE2, MOUSE2B, MOUSECOMBO, MAS2
    }
    public class Device
    {
        public Config Config { get; set; }
        public DeviceType deviceType { get; private set; }

        public Device(DeviceType deviceType)
        {
            this.deviceType = deviceType;
            switch(deviceType)
            {
                case DeviceBase.DeviceType.MOUSE2:
                    this.Config = new Config_MOUSE2();
                    break;
                case DeviceBase.DeviceType.MOUSE2B:
                    this.Config = new Config_MOUSE2B();
                    break;
                case DeviceBase.DeviceType.MOUSECOMBO:
                    this.Config = new Config_MOUSECOMBO();
                    break;
                case DeviceBase.DeviceType.MAS2:
                    this.Config = new Config_MAS2();
                    break;
            }
        }

        public string GetDataMessage()
        {
            switch (deviceType)
            {
                case DeviceBase.DeviceType.MOUSE2:
                    return generateMOUSE2Data();
                    break;
                case DeviceBase.DeviceType.MOUSE2B:
                    return generateMOUSE2BData();
                    break;
                case DeviceBase.DeviceType.MOUSECOMBO:
                    return generateMOUSECOMBOData();
                    break;
                case DeviceBase.DeviceType.MAS2:
                    return generateMAS2Data();
                    break;
            }
            return "";
        }

        private string generateMOUSE2Data()
        {
            DeviceData_MOUSE2 data = new DeviceData_MOUSE2();

            data.Voltage = new Normal(5, 3).Sample();
            data.Resistance = new Normal(0.5, 0.25).Sample();

            var message = new { Sender = Config.UniqueId, Name = Config.Name, Timestamp = DateTime.Now, Data = data };

            return JsonSerializer.Serialize(message, new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals });
        }
        private string generateMOUSE2BData()
        {
            DeviceData_MOUSE2B data = new DeviceData_MOUSE2B();

            data.Voltage = new Normal(5, 3).Sample();
            data.Resistance = new Normal(0.5, 0.25).Sample();
            data.LeakLocation = new Random().NextDouble();

            var message = new { Sender = Config.UniqueId, Name = Config.Name, Timestamp = DateTime.Now, Data = data };

            return JsonSerializer.Serialize(message, new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals });
        }
        private string generateMOUSECOMBOData()
        {
            DeviceData_MOUSECOMBO data = new DeviceData_MOUSECOMBO();

            data.Voltage = new Normal(5, 3).Sample();
            data.Resistance = new Normal(0.5, 0.25).Sample();

            for(int i = 0; i < 20; i++)
            {
                var reflectogram = new Reflectogram();
                reflectogram.SeriesNumber = (byte)i;
                List<double> values = new List<double>();
                for (int y = 0; y < 20; y++)
                {
                    values.Add(new Normal(0, 0.1).Sample());
                }
                reflectogram.Data = JsonSerializer.SerializeToUtf8Bytes(values);
                data.Reflectograms.Add(reflectogram);
            }

            var message = new { Sender = Config.UniqueId, Name = Config.Name, Timestamp = DateTime.Now, Data = data };

            return JsonSerializer.Serialize(message, new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals });
        }
        private string generateMAS2Data()
        {
            DeviceData_MAS2 data = new DeviceData_MAS2();

            data.Humidity = Math.Clamp(new Normal(50, 5).Sample(), 0, 100);
            data.Temperature = new Normal(20, 5).Sample();

            var message = new { Sender = Config.UniqueId, Name = Config.Name, Timestamp = DateTime.Now, Data = data };

            return JsonSerializer.Serialize(message, new JsonSerializerOptions { NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals });
        }
    }
}
