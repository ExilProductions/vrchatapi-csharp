/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class UnsubscribedEvent
    {
        [JsonProperty("topic")] public string Topic { get; set; }
    }
}
