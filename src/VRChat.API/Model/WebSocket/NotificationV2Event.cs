/*
 * VRChat API Documentation - WebSocket Support
 */

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class NotificationV2Event
    {
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("version")] public int Version { get; set; }
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("category")] public string Category { get; set; }
        [JsonProperty("isSystem")] public bool? IsSystem { get; set; }
        [JsonProperty("ignoreDND")] public bool? IgnoreDND { get; set; }
        [JsonProperty("senderUserId")] public string SenderUserId { get; set; }
        [JsonProperty("senderUsername")] public string SenderUsername { get; set; }
        [JsonProperty("receiverUserId")] public string ReceiverUserId { get; set; }
        [JsonProperty("relatedNotificationsId")] public string RelatedNotificationsId { get; set; }
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("message")] public string Message { get; set; }
        [JsonProperty("imageUrl")] public string ImageUrl { get; set; }
        [JsonProperty("link")] public string Link { get; set; }
        [JsonProperty("linkText")] public string LinkText { get; set; }
        [JsonProperty("responses")] public List<NotificationV2Response> Responses { get; set; }
        [JsonProperty("expiresAt")] public DateTime? ExpiresAt { get; set; }
        [JsonProperty("expiryAfterSeen")] public int? ExpiryAfterSeen { get; set; }
        [JsonProperty("requireSeen")] public bool? RequireSeen { get; set; }
        [JsonProperty("seen")] public bool? Seen { get; set; }
        [JsonProperty("canDelete")] public bool? CanDelete { get; set; }
        [JsonProperty("createdAt")] public DateTime? CreatedAt { get; set; }
        [JsonProperty("updatedAt")] public DateTime? UpdatedAt { get; set; }
    }

    public sealed class NotificationV2Response
    {
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("data")] public string Data { get; set; }
        [JsonProperty("icon")] public string Icon { get; set; }
        [JsonProperty("text")] public string Text { get; set; }
    }
}
