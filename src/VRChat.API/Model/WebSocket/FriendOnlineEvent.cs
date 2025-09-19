/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class FriendOnlineEvent
    {
        [JsonProperty("userId")] public string UserId { get; set; }
        [JsonProperty("displayName")] public string DisplayName { get; set; }
        [JsonProperty("platform")] public string Platform { get; set; }
        [JsonProperty("location")] public string Location { get; set; }
        [JsonProperty("worldId")] public string WorldId { get; set; }
        [JsonProperty("instanceId")] public string InstanceId { get; set; }
    }
}
