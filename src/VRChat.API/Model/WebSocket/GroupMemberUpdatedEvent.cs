/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;
using VRChat.API.Model;

namespace VRChat.API.Model.WebSocket
{
    public sealed class GroupMemberUpdatedEvent
    {
        [JsonProperty("member")] public GroupLimitedMember Member { get; set; }
    }
}
