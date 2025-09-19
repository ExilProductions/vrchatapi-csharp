/*
 * VRChat API Documentation - WebSocket Support
 */

using System;
using System.Net.WebSockets;

namespace VRChat.API.Client
{
    public sealed class SocketClosedEventArgs : EventArgs
    {
        public WebSocketCloseStatus? CloseStatus { get; }
        public string CloseStatusDescription { get; }
        public bool WillReconnect { get; }

        public SocketClosedEventArgs(WebSocketCloseStatus? status, string description, bool willReconnect)
        {
            CloseStatus = status;
            CloseStatusDescription = description;
            WillReconnect = willReconnect;
        }
    }
}
