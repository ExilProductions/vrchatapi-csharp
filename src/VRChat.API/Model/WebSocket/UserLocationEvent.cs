/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class UserLocationEvent
    {
        [JsonProperty("userId")] public string UserId { get; set; }
        [JsonProperty("location")] public string Location { get; set; }
        [JsonProperty("instance")] public string Instance { get; set; }
        [JsonProperty("travelingToLocation")] public string TravelingToLocation { get; set; }
    }
}
