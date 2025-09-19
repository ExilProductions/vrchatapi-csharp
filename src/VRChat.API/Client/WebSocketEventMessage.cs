/*
 * VRChat API Documentation - WebSocket Support
 */

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VRChat.API.Client
{
    /// <summary>
    /// Represents a generic event envelope from the VRChat WebSocket.
    /// See https://vrchat.community/websocket for current schema. We keep this liberal to avoid breakage.
    /// </summary>
    public sealed class WebSocketEventMessage
    {
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("topic")] public string Topic { get; set; }
        [JsonProperty("data")] public JToken Data { get; set; }
        [JsonProperty("content")] public JToken Content { get; set; }
        [JsonProperty("timestamp")] public DateTimeOffset? Timestamp { get; set; }
        [JsonProperty("id")] public string Id { get; set; }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            if (Data == null && Content != null)
            {
                Data = Content;
            }
        }

        /// <summary>
        /// Convert Data to a concrete type.
        /// </summary>
        public T DataAs<T>()
        {
            return Data == null ? default(T) : Data.ToObject<T>();
        }

        public override string ToString()
        {
            return $"{Type} {Topic} {Timestamp?.ToString("o")}";
        }
    }
}
