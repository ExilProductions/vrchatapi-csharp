/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class FriendRequestEvent
    {
        [JsonProperty("senderUserId")] public string SenderUserId { get; set; }
        [JsonProperty("senderDisplayName")] public string SenderDisplayName { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
    }
}
