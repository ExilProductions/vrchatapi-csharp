/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class FriendStatusUpdateEvent
    {
        [JsonProperty("userId")] public string UserId { get; set; }
        [JsonProperty("displayName")] public string DisplayName { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("statusDescription")] public string StatusDescription { get; set; }
    }
}
