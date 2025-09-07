namespace Shared
{
    public class RpcConfigMessage
    {
        public RpcConfigMessageType Type { get; set; }
        public string JsonMessage { get; set; } = "";
    }
    public enum RpcConfigMessageType
    {
        GET_CONFIG, UPDATE_CONFIG
    }
}