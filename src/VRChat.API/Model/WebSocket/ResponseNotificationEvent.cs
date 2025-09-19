/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class ResponseNotificationEvent
    {
        [JsonProperty("notificationId")] public string NotificationId { get; set; }
        [JsonProperty("receiverId")] public string ReceiverId { get; set; }
        [JsonProperty("responseId")] public string ResponseId { get; set; }
    }
}
