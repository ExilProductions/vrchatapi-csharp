/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class FriendActiveEvent
    {
        [JsonProperty("userid")] public string UserId { get; set; }
        [JsonProperty("platform")] public string Platform { get; set; }
    }
}
