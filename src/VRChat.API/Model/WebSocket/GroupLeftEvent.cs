/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class GroupLeftEvent
    {
        [JsonProperty("groupId")] public string GroupId { get; set; }
    }
}
