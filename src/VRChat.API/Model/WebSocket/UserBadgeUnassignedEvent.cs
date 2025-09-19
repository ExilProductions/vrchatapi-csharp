/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class UserBadgeUnassignedEvent
    {
        [JsonProperty("badgeId")] public string BadgeId { get; set; }
    }
}
