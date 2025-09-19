/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class InstanceQueueJoinedEvent
    {
        [JsonProperty("instanceLocation")] public string InstanceLocation { get; set; }
        [JsonProperty("position")] public int? Position { get; set; }
    }
}
