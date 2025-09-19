/*
 * VRChat API Documentation - WebSocket Support
 */

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VRChat.API.Model.WebSocket
{
    public sealed class NotificationV2UpdateEvent
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("version")] public int Version { get; set; }
        [JsonProperty("updates")] public IDictionary<string, JToken> Updates { get; set; }
    }
}
