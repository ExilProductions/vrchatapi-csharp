/*
 * VRChat API Documentation - WebSocket Support
 */

using System.Collections.Generic;
using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class UserUpdateEvent
    {
        [JsonProperty("userId")] public string UserId { get; set; }

        [JsonProperty("user")] public UserUpdateUser User { get; set; }
    }

    public sealed class UserUpdateUser
    {
        [JsonProperty("bio")] public string Bio { get; set; }
        [JsonProperty("currentAvatar")] public string CurrentAvatar { get; set; }
        [JsonProperty("currentAvatarAssetUrl")] public string CurrentAvatarAssetUrl { get; set; }
        [JsonProperty("currentAvatarImageUrl")] public string CurrentAvatarImageUrl { get; set; }
        [JsonProperty("currentAvatarThumbnailImageUrl")] public string CurrentAvatarThumbnailImageUrl { get; set; }
        [JsonProperty("displayName")] public string DisplayName { get; set; }
        [JsonProperty("fallbackAvatar")] public string FallbackAvatar { get; set; }
        [JsonProperty("id")] public string Id { get; set; }
        [JsonProperty("profilePicOverride")] public string ProfilePicOverride { get; set; }
        [JsonProperty("status")] public string Status { get; set; }
        [JsonProperty("statusDescription")] public string StatusDescription { get; set; }
        [JsonProperty("tags")] public List<string> Tags { get; set; }
        [JsonProperty("userIcon")] public string UserIcon { get; set; }
        [JsonProperty("username")] public string Username { get; set; }
    }
}
