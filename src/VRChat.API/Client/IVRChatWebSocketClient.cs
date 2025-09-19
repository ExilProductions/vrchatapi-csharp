/*
 * VRChat API Documentation - WebSocket Support
 */

using System;
using System.Threading;
using System.Threading.Tasks;
using VRChat.API.Model;
using VRChat.API.Model.WebSocket;

namespace VRChat.API.Client
{
    /// <summary>
    /// A minimal interface to interact with VRChat realtime websocket events.
    /// </summary>
    public interface IVRChatWebSocketClient : IDisposable
    {
        /// <summary>
        /// Connect to the VRChat websocket.
        /// </summary>
        Task ConnectAsync(CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Disconnect gracefully.
        /// </summary>
        Task DisconnectAsync(string reason = null, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// True if the socket is connected.
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// Send a raw text message (if the API ever requires client->server messages).
        /// </summary>
        Task SendAsync(string message, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Subscribe to a specific topic according to the VRChat websocket API.
        /// This simply sends a JSON command: { "type": "subscribe", "topic": topic }
        /// </summary>
        Task SubscribeAsync(string topic, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Unsubscribe from a specific topic.
        /// Sends: { "type": "unsubscribe", "topic": topic }
        /// </summary>
        Task UnsubscribeAsync(string topic, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Occurs when the connection is opened.
        /// </summary>
        event EventHandler Connected;

        /// <summary>
        /// Occurs when the connection is closed.
        /// </summary>
        event EventHandler<SocketClosedEventArgs> Disconnected;

        /// <summary>
        /// Occurs when an error occurs.
        /// </summary>
        event EventHandler<Exception> Error;

        /// <summary>
        /// Occurs when a raw JSON message is received.
        /// </summary>
        event EventHandler<string> MessageReceived;

        /// <summary>
        /// Occurs when a deserialized typed message is received.
        /// </summary>
        event EventHandler<WebSocketEventMessage> EventReceived;

        // Typed user/friend presence & status
        event EventHandler<WebSocketEvent<UserOnlineEvent>> UserOnline;
        event EventHandler<WebSocketEvent<UserOfflineEvent>> UserOffline;
        event EventHandler<WebSocketEvent<FriendOnlineEvent>> FriendOnline;
        event EventHandler<WebSocketEvent<FriendOfflineEvent>> FriendOffline;
        event EventHandler<WebSocketEvent<FriendLocationEvent>> FriendLocation;
        event EventHandler<WebSocketEvent<FriendAddEvent>> FriendAdd;
        event EventHandler<WebSocketEvent<FriendDeleteEvent>> FriendDelete;
        event EventHandler<WebSocketEvent<FriendStatusUpdateEvent>> FriendStatusUpdate;
        event EventHandler<WebSocketEvent<FriendActiveEvent>> FriendActive;
        event EventHandler<WebSocketEvent<FriendUpdateEvent>> FriendUpdate;

        // Typed notifications/invites
        event EventHandler<WebSocketEvent<FriendRequestEvent>> FriendRequest;
        event EventHandler<WebSocketEvent<InviteEvent>> Invite;
        event EventHandler<WebSocketEvent<InviteResponseEvent>> InviteResponse;
        event EventHandler<WebSocketEvent<RequestInviteEvent>> RequestInvite;
        event EventHandler<WebSocketEvent<RequestInviteResponseEvent>> RequestInviteResponse;
        event EventHandler<WebSocketEvent<NotificationEvent>> Notification;
        event EventHandler<WebSocketEvent<ResponseNotificationEvent>> ResponseNotification;
        event EventHandler<WebSocketEvent<SeeNotificationEvent>> SeeNotification;
        event EventHandler<WebSocketEvent<HideNotificationEvent>> HideNotification;
        event EventHandler<WebSocketEvent<ClearNotificationEvent>> ClearNotification;
        event EventHandler<WebSocketEvent<NotificationV2Event>> NotificationV2;
        event EventHandler<WebSocketEvent<NotificationV2UpdateEvent>> NotificationV2Update;
        event EventHandler<WebSocketEvent<NotificationV2DeleteEvent>> NotificationV2Delete;
        event EventHandler<WebSocketEvent<NotificationSeenEvent>> NotificationSeen;

        // User
        event EventHandler<WebSocketEvent<UserUpdateEvent>> UserUpdate;
        event EventHandler<WebSocketEvent<UserLocationEvent>> UserLocation;
        event EventHandler<WebSocketEvent<UserBadgeAssignedEvent>> UserBadgeAssigned;
        event EventHandler<WebSocketEvent<UserBadgeUnassignedEvent>> UserBadgeUnassigned;
        event EventHandler<WebSocketEvent<ContentRefreshEvent>> ContentRefresh;
        event EventHandler<WebSocketEvent<ModifiedImageUpdateEvent>> ModifiedImageUpdate;
        event EventHandler<WebSocketEvent<InstanceQueueJoinedEvent>> InstanceQueueJoined;
        event EventHandler<WebSocketEvent<InstanceQueueReadyEvent>> InstanceQueueReady;

        // Groups
        event EventHandler<WebSocketEvent<GroupJoinedEvent>> GroupJoined;
        event EventHandler<WebSocketEvent<GroupLeftEvent>> GroupLeft;
        event EventHandler<WebSocketEvent<GroupMemberUpdatedEvent>> GroupMemberUpdated;
        event EventHandler<WebSocketEvent<GroupRoleUpdatedEvent>> GroupRoleUpdated;

        // System/control
        event EventHandler<WebSocketEvent<HelloEvent>> Hello;
        event EventHandler<WebSocketEvent<SubscribedEvent>> Subscribed;
        event EventHandler<WebSocketEvent<UnsubscribedEvent>> Unsubscribed;
        event EventHandler<WebSocketEvent<ErrorEvent>> ServerError;
    }
}
