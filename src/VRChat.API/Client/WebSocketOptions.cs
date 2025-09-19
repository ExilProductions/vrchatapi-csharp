/*
 * VRChat API Documentation - WebSocket Support
 */

using System;
using System.Collections.Generic;
using System.Net;

namespace VRChat.API.Client
{
    /// <summary>
    /// Options for configuring the VRChat WebSocket client.
    /// </summary>
    public sealed class WebSocketOptions
    {
        /// <summary>
        /// Optional explicit endpoint. If null, it will be derived from the configured BasePath by switching scheme to ws/wss
        /// and using root path '/'.
        /// </summary>
        public Uri Endpoint { get; set; }

        /// <summary>
        /// Optional additional headers to include in the WebSocket handshake.
        /// </summary>
        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Optional WebSocket subprotocols to request.
        /// </summary>
        public IList<string> SubProtocols { get; } = new List<string>();

        /// <summary>
        /// The size of the receive buffer for frames. Default is 8 KB.
        /// </summary>
        public int ReceiveBufferSize { get; set; } = 8 * 1024;

        /// <summary>
        /// Interval for WebSocket pings. Default is 30 seconds.
        /// </summary>
        public TimeSpan KeepAliveInterval { get; set; } = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Enable automatic reconnection when the socket closes or errors. Default true.
        /// </summary>
        public bool AutoReconnect { get; set; } = true;

        /// <summary>
        /// Maximum reconnect delay. Default 60 seconds.
        /// </summary>
        public TimeSpan MaxReconnectDelay { get; set; } = TimeSpan.FromSeconds(60);

        /// <summary>
        /// Initial reconnect delay. Default 1 second.
        /// </summary>
        public TimeSpan InitialReconnectDelay { get; set; } = TimeSpan.FromSeconds(1);

        /// <summary>
        /// Maximum text message size to accumulate before raising OnMessage (to protect against unbounded memory). Default 8 MB.
        /// </summary>
        public int MaxMessageBytes { get; set; } = 8 * 1024 * 1024;

        /// <summary>
        /// Optional proxy to use. If null, uses configuration proxy if available.
        /// </summary>
        public IWebProxy Proxy { get; set; }

        /// <summary>
        /// Optional endpoint path (e.g., "/") if Endpoint is not set and BasePath contains extra path you want to override.
        /// Default is "/".
        /// </summary>
        public string DefaultPath { get; set; } = "/";

        /// <summary>
        /// Optional factory to provide time for testing.
        /// </summary>
        internal Func<DateTimeOffset> NowProvider { get; set; } = () => DateTimeOffset.UtcNow;
    }
}
