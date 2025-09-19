/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class UserOfflineEvent
    {
        [JsonProperty("userId")] public string UserId { get; set; }
        [JsonProperty("displayName")] public string DisplayName { get; set; }
        [JsonProperty("lastPlatform")] public string LastPlatform { get; set; }
        [JsonProperty("lastLocation")] public string LastLocation { get; set; }
    }
}
