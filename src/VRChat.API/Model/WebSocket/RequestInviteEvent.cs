/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class RequestInviteEvent
    {
        [JsonProperty("requesterUserId")] public string RequesterUserId { get; set; }
        [JsonProperty("requesterDisplayName")] public string RequesterDisplayName { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
    }
}
