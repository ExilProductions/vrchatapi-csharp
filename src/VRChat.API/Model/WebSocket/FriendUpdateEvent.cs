/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class FriendUpdateEvent
    {
        [JsonProperty("userId")] public string UserId { get; set; }
    }
}
