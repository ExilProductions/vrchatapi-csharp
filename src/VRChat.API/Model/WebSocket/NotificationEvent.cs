/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class NotificationEvent
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("senderUserId")] public string SenderUserId { get; set; }
        [JsonProperty("receiverUserId")] public string ReceiverUserId { get; set; }
        [JsonProperty("details")] public object Details { get; set; }
    }
}
