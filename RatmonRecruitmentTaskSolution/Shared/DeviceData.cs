using System;
using System.Collections.Generic;
using System.Text;

namespace Shared
{
    public abstract class DeviceData
    {

    }

    public class DeviceData_MOUSE2 : DeviceData
    {
        public double Voltage { get; set; }
        public double Resistance { get; set; }
    }
    public class DeviceData_MOUSE2B : DeviceData
    {
        public double Voltage { get; set; }
        public double Resistance { get; set; }
        public double LeakLocation { get; set; }
    }
    public class DeviceData_MOUSECOMBO : DeviceData
    {
        public double Voltage { get; set; }
        public double Resistance { get; set; }
        public List<Reflectogram> Reflectograms { get; set; } = new List<Reflectogram>();
    }
    public class DeviceData_MAS2 : DeviceData
    {
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
