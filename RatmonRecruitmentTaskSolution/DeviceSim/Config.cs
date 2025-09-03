using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceBase
{
    public abstract class Config
    {
        public string UniqueId { get; set; }
        public string Name { get; set; }
    }

    public class Config_MOUSE2 : Config
    {
        public double AlarmThreshold { get; set; }
    }

    public class Config_MOUSE2B : Config
    {
        public double AlarmThreshold { get; set; }
        public double CableLength { get; set; }
    }

    public class Config_MOUSECOMBO : Config
    {
        public double AlarmThreshold { get; set; }
    }

    public class Config_MAS2 : Config
    {
        public double TemperatureThreshold { get; set; }
        public double HumidityThreshold { get; set; }
    }
}
