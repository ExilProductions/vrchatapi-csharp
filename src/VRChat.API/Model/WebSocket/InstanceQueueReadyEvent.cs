/*
 * VRChat API Documentation - WebSocket Support
 */

using System;
using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class InstanceQueueReadyEvent
    {
        [JsonProperty("instanceLocation")] public string InstanceLocation { get; set; }
        [JsonProperty("expiry")] public DateTime? Expiry { get; set; }
    }
}
