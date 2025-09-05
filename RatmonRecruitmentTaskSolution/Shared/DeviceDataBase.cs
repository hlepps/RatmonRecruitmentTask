using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace Shared
{
    /// <summary>
    /// Base class for actual data from devices
    /// </summary>
    public class DeviceDataBase
    {
        [JsonIgnore]
        public int Id { get; set; }
    }

    public class DeviceData_MOUSE2 : DeviceDataBase
    {
        public double Voltage { get; set; }
        public double Resistance { get; set; }

        public override string ToString()
        {
            return $"Voltage:{Voltage} Resistance:{Resistance}";
        }
    }
    public class DeviceData_MOUSE2B : DeviceDataBase
    {
        public double Voltage { get; set; }
        public double Resistance { get; set; }
        public double LeakLocation { get; set; }
        public override string ToString()
        {
            return $"Voltage:{Voltage} Resistance:{Resistance} LeakLocation:{LeakLocation}";
        }
    }
    public class DeviceData_MOUSECOMBO : DeviceDataBase
    {
        public double Voltage { get; set; }
        public double Resistance { get; set; }
        public List<Reflectogram> Reflectograms { get; set; } = new List<Reflectogram>();
        public override string ToString()
        {
            return $"Voltage:{Voltage} Resistance:{Resistance} List<Reflectogram>:{Reflectograms.Count}";
        }
    }
    public class DeviceData_MAS2 : DeviceDataBase
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
        public override string ToString()
        {
            return $"Temperature:{Temperature} Humidity:{Humidity}";
        }
    }
}
