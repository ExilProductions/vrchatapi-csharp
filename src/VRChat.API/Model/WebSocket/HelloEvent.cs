/*
 * VRChat API Documentation - WebSocket Support
 */

using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VRChat.API.Model.WebSocket
{
    public sealed class HelloEvent
    {
        [JsonProperty("hello")] public string Hello { get; set; }
        [JsonProperty("sessionId")] public string SessionId { get; set; }
        [JsonProperty("serverTime")] public string ServerTime { get; set; }
        [JsonProperty("subscriptions")] public IList<string> Subscriptions { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData { get; set; }
    }
}
