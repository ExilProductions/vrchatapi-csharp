/*
 * VRChat API Documentation - WebSocket Support
 */

using System.Collections.Generic;
using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class NotificationV2DeleteEvent
    {
        [JsonProperty("ids")] public List<string> Ids { get; set; }
        [JsonProperty("version")] public int Version { get; set; }
    }
}
