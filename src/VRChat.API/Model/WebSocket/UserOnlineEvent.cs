/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class UserOnlineEvent
    {
        [JsonProperty("userId")] public string UserId { get; set; }
        [JsonProperty("displayName")] public string DisplayName { get; set; }
        [JsonProperty("platform")] public string Platform { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("location")] public string Location { get; set; }
    }
}
