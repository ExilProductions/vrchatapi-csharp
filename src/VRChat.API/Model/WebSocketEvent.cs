/*
 * VRChat API Documentation - WebSocket Support
 */

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VRChat.API.Model
{
    /// <summary>
    /// Generic typed WebSocket event envelope.
    /// Supports both 'data' and 'content' payload keys from the server.
    /// </summary>
    public sealed class WebSocketEvent<T>
    {
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("topic")] public string Topic { get; set; }

        [JsonProperty("data")] public T Data { get; set; }
        [JsonProperty("content")] public T Content { get; set; }

        [JsonProperty("timestamp")] public DateTimeOffset? Timestamp { get; set; }
        [JsonProperty("id")] public string Id { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (Equals(Data, default(T)) && !Equals(Content, default(T)))
            {
                Data = Content;
            }
        }
    }
}
