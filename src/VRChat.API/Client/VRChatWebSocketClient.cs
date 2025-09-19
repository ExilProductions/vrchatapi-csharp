/*
 * VRChat API Documentation - WebSocket Support
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VRChat.API.Model;
using VRChat.API.Model.WebSocket;

namespace VRChat.API.Client
{
    /// <summary>
    /// Production-ready WebSocket client for VRChat realtime events.
    /// - Cookie-based auth using the existing ApiClient cookie jar
    /// - Auto-reconnect with backoff
    /// - Ping/keepalive
    /// - Thread-safe send and graceful shutdown
    /// - Raw and typed JSON message events
    /// </summary>
    public sealed class VRChatWebSocketClient : IVRChatWebSocketClient
    {
        private readonly IReadableConfiguration _configuration;
        private readonly WebSocketOptions _options;
        private ClientWebSocket _socket;
        private readonly object _stateLock = new object();
        private readonly SemaphoreSlim _sendGate = new SemaphoreSlim(1, 1);
        private CancellationTokenSource _lifecycleCts;
        private Task _receiveLoopTask;
        private volatile bool _disposed;
        private volatile bool _userInitiatedClose;

        public event EventHandler Connected;
        public event EventHandler<SocketClosedEventArgs> Disconnected;
        public event EventHandler<string> MessageReceived;
        public event EventHandler<WebSocketEventMessage> EventReceived;
        public event EventHandler<Exception> Error;

        // Friends/presence
        public event EventHandler<WebSocketEvent<UserOnlineEvent>> UserOnline;
        public event EventHandler<WebSocketEvent<UserOfflineEvent>> UserOffline;
        public event EventHandler<WebSocketEvent<FriendOnlineEvent>> FriendOnline;
        public event EventHandler<WebSocketEvent<FriendOfflineEvent>> FriendOffline;
        public event EventHandler<WebSocketEvent<FriendLocationEvent>> FriendLocation;
        public event EventHandler<WebSocketEvent<FriendAddEvent>> FriendAdd;
        public event EventHandler<WebSocketEvent<FriendDeleteEvent>> FriendDelete;
        public event EventHandler<WebSocketEvent<FriendStatusUpdateEvent>> FriendStatusUpdate;
        public event EventHandler<WebSocketEvent<FriendActiveEvent>> FriendActive;
        public event EventHandler<WebSocketEvent<FriendUpdateEvent>> FriendUpdate;

        // Notifications
        public event EventHandler<WebSocketEvent<FriendRequestEvent>> FriendRequest;
        public event EventHandler<WebSocketEvent<InviteEvent>> Invite;
        public event EventHandler<WebSocketEvent<InviteResponseEvent>> InviteResponse;
        public event EventHandler<WebSocketEvent<RequestInviteEvent>> RequestInvite;
        public event EventHandler<WebSocketEvent<RequestInviteResponseEvent>> RequestInviteResponse;
        public event EventHandler<WebSocketEvent<NotificationEvent>> Notification;
        public event EventHandler<WebSocketEvent<ResponseNotificationEvent>> ResponseNotification;
        public event EventHandler<WebSocketEvent<SeeNotificationEvent>> SeeNotification;
        public event EventHandler<WebSocketEvent<HideNotificationEvent>> HideNotification;
        public event EventHandler<WebSocketEvent<ClearNotificationEvent>> ClearNotification;
        public event EventHandler<WebSocketEvent<NotificationV2Event>> NotificationV2;
        public event EventHandler<WebSocketEvent<NotificationV2UpdateEvent>> NotificationV2Update;
        public event EventHandler<WebSocketEvent<NotificationV2DeleteEvent>> NotificationV2Delete;
        public event EventHandler<WebSocketEvent<NotificationSeenEvent>> NotificationSeen;

        // User
        public event EventHandler<WebSocketEvent<UserUpdateEvent>> UserUpdate;
        public event EventHandler<WebSocketEvent<UserLocationEvent>> UserLocation;
        public event EventHandler<WebSocketEvent<UserBadgeAssignedEvent>> UserBadgeAssigned;
        public event EventHandler<WebSocketEvent<UserBadgeUnassignedEvent>> UserBadgeUnassigned;
        public event EventHandler<WebSocketEvent<ContentRefreshEvent>> ContentRefresh;
        public event EventHandler<WebSocketEvent<ModifiedImageUpdateEvent>> ModifiedImageUpdate;
        public event EventHandler<WebSocketEvent<InstanceQueueJoinedEvent>> InstanceQueueJoined;
        public event EventHandler<WebSocketEvent<InstanceQueueReadyEvent>> InstanceQueueReady;

        // Groups
        public event EventHandler<WebSocketEvent<GroupJoinedEvent>> GroupJoined;
        public event EventHandler<WebSocketEvent<GroupLeftEvent>> GroupLeft;
        public event EventHandler<WebSocketEvent<GroupMemberUpdatedEvent>> GroupMemberUpdated;
        public event EventHandler<WebSocketEvent<GroupRoleUpdatedEvent>> GroupRoleUpdated;

        // System/control
        public event EventHandler<WebSocketEvent<HelloEvent>> Hello;
        public event EventHandler<WebSocketEvent<SubscribedEvent>> Subscribed;
        public event EventHandler<WebSocketEvent<UnsubscribedEvent>> Unsubscribed;
        public event EventHandler<WebSocketEvent<ErrorEvent>> ServerError;

        public bool IsConnected
        {
            get
            {
                var s = _socket;
                return s != null && s.State == WebSocketState.Open;
            }
        }

        public VRChatWebSocketClient(IReadableConfiguration configuration, WebSocketOptions options = null)
        {
            _configuration = configuration ?? GlobalConfiguration.Instance;
            _options = options ?? new WebSocketOptions();
        }

        public async Task ConnectAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            lock (_stateLock)
            {
                if (_socket != null && _socket.State == WebSocketState.Open)
                {
                    return; // already connected
                }
                _userInitiatedClose = false;
                _lifecycleCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            }

            await ConnectInnerAsync(_lifecycleCts.Token).ConfigureAwait(false);
        }

        public async Task DisconnectAsync(string reason = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            ClientWebSocket socket;
            Task receiveTask;
            lock (_stateLock)
            {
                _userInitiatedClose = true;
                socket = _socket;
                receiveTask = _receiveLoopTask;
                _lifecycleCts?.Cancel();
            }

            if (socket != null && (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived))
            {
                try
                {
                    await socket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, reason ?? "client_shutdown", cancellationToken).ConfigureAwait(false);
                }
                catch { /* ignore */ }
            }

            if (receiveTask != null)
            {
                try { await receiveTask.ConfigureAwait(false); } catch { /* ignore */ }
            }

            lock (_stateLock)
            {
                socket?.Dispose();
                _socket = null;
                _receiveLoopTask = null;
                _lifecycleCts?.Dispose();
                _lifecycleCts = null;
            }
        }

        public async Task SendAsync(string message, CancellationToken cancellationToken = default(CancellationToken))
        {
            ThrowIfDisposed();
            if (message == null) throw new ArgumentNullException("message");

            var socket = _socket;
            if (socket == null || socket.State != WebSocketState.Open)
            {
                throw new InvalidOperationException("WebSocket is not connected");
            }

            var bytes = Encoding.UTF8.GetBytes(message);
            await _sendGate.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                var segment = new ArraySegment<byte>(bytes);
                await socket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                _sendGate.Release();
            }
        }

        public Task SubscribeAsync(string topic, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentNullException("topic");
            var payload = JsonConvert.SerializeObject(new { type = "subscribe", topic = topic });
            return SendAsync(payload, cancellationToken);
        }

        public Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(topic)) throw new ArgumentNullException("topic");
            var payload = JsonConvert.SerializeObject(new { type = "unsubscribe", topic = topic });
            return SendAsync(payload, cancellationToken);
        }

        private async Task ConnectInnerAsync(CancellationToken cancellationToken)
        {
            var endpoint = ResolveEndpoint();
            var socket = new ClientWebSocket();

            // KeepAlive
            socket.Options.KeepAliveInterval = _options.KeepAliveInterval;

            // Cookies from ApiClient HTTP cookie container for auth continuity
            try
            {
                socket.Options.Cookies = ApiClient.CookieContainer;
            }
            catch { /* some platforms may throw if not supported; ignore */ }

            // Proxy
            try
            {
                var proxy = _options.Proxy ?? _configuration.Proxy;
                if (proxy != null)
                {
                    socket.Options.Proxy = proxy;
                }
            }
            catch { /* ignore */ }

            // Subprotocols
            if (_options.SubProtocols != null && _options.SubProtocols.Count > 0)
            {
                foreach (var sp in _options.SubProtocols)
                {
                    if (!string.IsNullOrWhiteSpace(sp)) socket.Options.AddSubProtocol(sp);
                }
            }
            else
            {
                // Request json as a sensible default if server supports it
                socket.Options.AddSubProtocol("json");
            }

            // Note: ClientWebSocketOptions in netstandard2.0 does not support setting arbitrary headers.
            // Authentication should flow via Cookies set above.

            // Connect
            await socket.ConnectAsync(endpoint, cancellationToken).ConfigureAwait(false);

            lock (_stateLock)
            {
                _socket = socket;
            }

            // fire connected
            SafeFire(() => Connected?.Invoke(this, EventArgs.Empty));

            // start receive loop
            _receiveLoopTask = Task.Run(() => ReceiveLoopAsync(endpoint, socket, cancellationToken));
        }

        private async Task ReceiveLoopAsync(Uri endpoint, ClientWebSocket socket, CancellationToken cancellationToken)
        {
            var buffer = new byte[Math.Max(1024, _options.ReceiveBufferSize)];
            var builder = new StringBuilder(Math.Min(_options.MaxMessageBytes, 64 * 1024));

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    builder.Length = 0;
                    WebSocketReceiveResult result;
                    do
                    {
                        var seg = new ArraySegment<byte>(buffer);
                        result = await socket.ReceiveAsync(seg, cancellationToken).ConfigureAwait(false);

                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            // graceful close from server
                            await HandleSocketClosedAsync(socket, result.CloseStatus, result.CloseStatusDescription).ConfigureAwait(false);
                            return;
                        }

                        var chunk = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        builder.Append(chunk);
                        if (builder.Length > _options.MaxMessageBytes)
                        {
                            throw new InvalidOperationException("Incoming WebSocket message exceeds MaxMessageBytes");
                        }
                    } while (!result.EndOfMessage);

                    var text = builder.ToString();
                    SafeFire(() => MessageReceived?.Invoke(this, text));

                    try
                    {
                        var envelope = JsonConvert.DeserializeObject<WebSocketEventMessage>(text);
                        if (envelope != null)
                        {
                            SafeFire(() => EventReceived?.Invoke(this, envelope));
                            TryEmitTyped(envelope, text);
                        }
                    }
                    catch (Exception ex)
                    {
                        SafeFire(() => Error?.Invoke(this, ex));
                    }
                }
                catch (OperationCanceledException)
                {
                    // normal exit
                    break;
                }
                catch (WebSocketException wse)
                {
                    // network error
                    await HandleSocketClosedAsync(socket, socket.CloseStatus, wse.Message).ConfigureAwait(false);
                    return;
                }
                catch (Exception ex)
                {
                    SafeFire(() => Error?.Invoke(this, ex));
                    await HandleSocketClosedAsync(socket, socket.CloseStatus, ex.Message).ConfigureAwait(false);
                    return;
                }
            }
        }

        private void TryEmitTyped(WebSocketEventMessage envelope, string raw)
        {
            // we normalize key for routing
            var key = (envelope.Type ?? envelope.Topic ?? string.Empty).ToLowerInvariant();

            try
            {
                switch (key)
                {
                    // System/control
                    case "hello":
                        Emit<HelloEvent>(raw, e => Hello?.Invoke(this, e));
                        break;
                    case "subscribed":
                        Emit<SubscribedEvent>(raw, e => Subscribed?.Invoke(this, e));
                        break;
                    case "unsubscribed":
                        Emit<UnsubscribedEvent>(raw, e => Unsubscribed?.Invoke(this, e));
                        break;
                    case "error":
                        Emit<ErrorEvent>(raw, e => ServerError?.Invoke(this, e));
                        break;

                    // Friend/presence events
                    case "user-online":
                        Emit<UserOnlineEvent>(raw, e => UserOnline?.Invoke(this, e));
                        break;
                    case "user-offline":
                        Emit<UserOfflineEvent>(raw, e => UserOffline?.Invoke(this, e));
                        break;
                    case "friend-online":
                        Emit<FriendOnlineEvent>(raw, e => FriendOnline?.Invoke(this, e));
                        break;
                    case "friend-active":
                        Emit<FriendActiveEvent>(raw, e => FriendActive?.Invoke(this, e));
                        break;
                    case "friend-offline":
                        Emit<FriendOfflineEvent>(raw, e => FriendOffline?.Invoke(this, e));
                        break;
                    case "friend-update":
                        Emit<FriendUpdateEvent>(raw, e => FriendUpdate?.Invoke(this, e));
                        break;
                    case "friend-location":
                        Emit<FriendLocationEvent>(raw, e => FriendLocation?.Invoke(this, e));
                        break;
                    case "friend-add":
                        Emit<FriendAddEvent>(raw, e => FriendAdd?.Invoke(this, e));
                        break;
                    case "friend-delete":
                        Emit<FriendDeleteEvent>(raw, e => FriendDelete?.Invoke(this, e));
                        break;
                    case "friend-status-update":
                        Emit<FriendStatusUpdateEvent>(raw, e => FriendStatusUpdate?.Invoke(this, e));
                        break;

                    // User events
                    case "user-update":
                        Emit<UserUpdateEvent>(raw, e => UserUpdate?.Invoke(this, e));
                        break;
                    case "user-location":
                        Emit<UserLocationEvent>(raw, e => UserLocation?.Invoke(this, e));
                        break;
                    case "user-badge-assigned":
                        Emit<UserBadgeAssignedEvent>(raw, e => UserBadgeAssigned?.Invoke(this, e));
                        break;
                    case "user-badge-unassigned":
                        Emit<UserBadgeUnassignedEvent>(raw, e => UserBadgeUnassigned?.Invoke(this, e));
                        break;
                    case "content-refresh":
                        Emit<ContentRefreshEvent>(raw, e => ContentRefresh?.Invoke(this, e));
                        break;
                    case "modified-image-update":
                        Emit<ModifiedImageUpdateEvent>(raw, e => ModifiedImageUpdate?.Invoke(this, e));
                        break;
                    case "instance-queue-joined":
                        Emit<InstanceQueueJoinedEvent>(raw, e => InstanceQueueJoined?.Invoke(this, e));
                        break;
                    case "instance-queue-ready":
                        Emit<InstanceQueueReadyEvent>(raw, e => InstanceQueueReady?.Invoke(this, e));
                        break;

                    // Group events
                    case "group-joined":
                        Emit<GroupJoinedEvent>(raw, e => GroupJoined?.Invoke(this, e));
                        break;
                    case "group-left":
                        Emit<GroupLeftEvent>(raw, e => GroupLeft?.Invoke(this, e));
                        break;
                    case "group-member-updated":
                        Emit<GroupMemberUpdatedEvent>(raw, e => GroupMemberUpdated?.Invoke(this, e));
                        break;
                    case "group-role-updated":
                        Emit<GroupRoleUpdatedEvent>(raw, e => GroupRoleUpdated?.Invoke(this, e));
                        break;

                    // Notifications v1
                    case "notification":
                        Emit<NotificationEvent>(raw, e => Notification?.Invoke(this, e));
                        break;
                    case "response-notification":
                        Emit<ResponseNotificationEvent>(raw, e => ResponseNotification?.Invoke(this, e));
                        break;
                    case "see-notification":
                        Emit<SeeNotificationEvent>(raw, e => SeeNotification?.Invoke(this, e));
                        break;
                    case "hide-notification":
                        Emit<HideNotificationEvent>(raw, e => HideNotification?.Invoke(this, e));
                        break;
                    case "clear-notification":
                        Emit<ClearNotificationEvent>(raw, e => ClearNotification?.Invoke(this, e));
                        break;

                    // Notifications v2
                    case "notification-v2":
                        Emit<NotificationV2Event>(raw, e => NotificationV2?.Invoke(this, e));
                        break;
                    case "notification-v2-update":
                        Emit<NotificationV2UpdateEvent>(raw, e => NotificationV2Update?.Invoke(this, e));
                        break;
                    case "notification-v2-delete":
                        Emit<NotificationV2DeleteEvent>(raw, e => NotificationV2Delete?.Invoke(this, e));
                        break;
                    case "notification-seen":
                        Emit<NotificationSeenEvent>(raw, e => NotificationSeen?.Invoke(this, e));
                        break;
                }
            }
            catch (Exception ex)
            {
                SafeFire(() => Error?.Invoke(this, ex));
            }
        }

        private void Emit<T>(string raw, Action<WebSocketEvent<T>> fire)
        {
            var e = JsonConvert.DeserializeObject<WebSocketEvent<T>>(raw);
            if (e != null)
            {
                SafeFire(() => fire(e));
            }
        }

        private async Task HandleSocketClosedAsync(ClientWebSocket socket, WebSocketCloseStatus? status, string description)
        {
            var auto = _options.AutoReconnect && !_userInitiatedClose && !_disposed;

            // raise event early to allow consumers to react before reconnect
            SafeFire(() => Disconnected?.Invoke(this, new SocketClosedEventArgs(status, description, auto)));

            lock (_stateLock)
            {
                try { socket?.Dispose(); } catch { /* ignore */ }
                if (ReferenceEquals(_socket, socket)) _socket = null;
            }

            if (auto)
            {
                await ReconnectLoopAsync().ConfigureAwait(false);
            }
        }

        private async Task ReconnectLoopAsync()
        {
            var delay = _options.InitialReconnectDelay;
            var max = _options.MaxReconnectDelay;
            var rand = new Random();

            while (!_disposed && !_userInitiatedClose)
            {
                try
                {
                    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                    await ConnectInnerAsync(cts.Token).ConfigureAwait(false);
                    return; // connected
                }
                catch (Exception ex)
                {
                    SafeFire(() => Error?.Invoke(this, ex));
                }

                // backoff with jitter
                var jitterMultiplier = 0.9 + (rand.NextDouble() * 0.2); // 0.9..1.1
                var doubledMs = delay.TotalMilliseconds * 2.0;
                var nextMs = Math.Min(max.TotalMilliseconds, doubledMs) * jitterMultiplier;
                var nextDelay = TimeSpan.FromMilliseconds(nextMs);

                await Task.Delay(delay, CancellationToken.None).ConfigureAwait(false);
                delay = nextDelay;
            }
        }

        private Uri ResolveEndpoint()
        {
            if (_options.Endpoint != null)
            {
                return _options.Endpoint;
            }

            // Try to infer a sensible default from BasePath
            // If API host is api.vrchat.cloud -> prefer pipeline.vrchat.cloud as per community docs
            Uri baseUri;
            if (!Uri.TryCreate(_configuration.BasePath, UriKind.Absolute, out baseUri))
            {
                throw new InvalidOperationException("Invalid configuration BasePath; set WebSocketOptions.Endpoint explicitly.");
            }

            string host = baseUri.Host;
            if (string.Equals(host, "api.vrchat.cloud", StringComparison.OrdinalIgnoreCase))
            {
                return new Uri("wss://pipeline.vrchat.cloud/");
            }

            // Fallback: change scheme to ws/wss and use root path or provided default path
            var scheme = string.Equals(baseUri.Scheme, "https", StringComparison.OrdinalIgnoreCase) ? "wss" : "ws";
            var builder = new UriBuilder(baseUri)
            {
                Scheme = scheme,
                Path = string.IsNullOrEmpty(_options.DefaultPath) ? "/" : _options.DefaultPath,
                Query = string.Empty
            };
            return builder.Uri;
        }

        private void SafeFire(Action action)
        {
            try { action(); } catch { /* ignore listener exceptions */ }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed) throw new ObjectDisposedException(GetType().Name);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            try { _lifecycleCts?.Cancel(); } catch { }
            try { _socket?.Dispose(); } catch { }
            _lifecycleCts?.Dispose();
            _sendGate?.Dispose();
        }
    }
}
