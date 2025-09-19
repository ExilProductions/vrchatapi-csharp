/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;
using VRChat.API.Model;

namespace VRChat.API.Model.WebSocket
{
    public sealed class GroupRoleUpdatedEvent
    {
        [JsonProperty("role")] public GroupRole Role { get; set; }
    }
}
