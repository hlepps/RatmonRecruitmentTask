using Shared;

namespace Server.Data.Models
{
    /// <summary>
    /// Database model
    /// </summary>
    public class DeviceData
    {
        public int Id { get; set; }
        public string DeviceId { get; set; }
        public DateTime Timestamp { get; set; }
        public DeviceDataBase Data { get; set; }
    }
}
