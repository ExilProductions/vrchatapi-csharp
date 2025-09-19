/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class NotificationSeenEvent
    {
        [JsonProperty("notificationId")] public string NotificationId { get; set; }
        [JsonProperty("userId")] public string UserId { get; set; }
        [JsonProperty("seen")] public bool Seen { get; set; }
    }
}
