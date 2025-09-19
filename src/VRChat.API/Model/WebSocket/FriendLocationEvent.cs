/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class FriendLocationEvent
    {
        [JsonProperty("userId")] public string UserId { get; set; }
        [JsonProperty("displayName")] public string DisplayName { get; set; }
        [JsonProperty("location")] public string Location { get; set; }
        [JsonProperty("worldId")] public string WorldId { get; set; }
        [JsonProperty("instanceId")] public string InstanceId { get; set; }
        [JsonProperty("travelingToLocation")] public string TravelingToLocation { get; set; }
    }
}
