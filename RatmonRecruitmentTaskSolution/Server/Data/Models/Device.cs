using Microsoft.EntityFrameworkCore;
using Shared;

namespace Server.Data.Models
{
    public class Device
    {
        public string Id { get; set; }
        public DeviceType Type { get; set; }
        public string Name { get; set; } = "";
        

    }
}
