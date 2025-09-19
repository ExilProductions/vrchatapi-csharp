/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class RequestInviteResponseEvent
    {
        [JsonProperty("responderUserId")] public string ResponderUserId { get; set; }
        [JsonProperty("responderDisplayName")] public string ResponderDisplayName { get; set; }
        [JsonProperty("accepted")] public bool? Accepted { get; set; }
        [JsonProperty("worldId")] public string WorldId { get; set; }
        [JsonProperty("instanceId")] public string InstanceId { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
    }
}
