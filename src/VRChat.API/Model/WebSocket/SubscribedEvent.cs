/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class SubscribedEvent
    {
        [JsonProperty("topic")] public string Topic { get; set; }
    }
}
