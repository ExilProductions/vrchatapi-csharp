/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class UserBadgeAssignedEvent
    {
        [JsonProperty("badge")] public string Badge { get; set; }
    }
}
