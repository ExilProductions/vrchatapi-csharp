/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class ContentRefreshEvent
    {
        [JsonProperty("contentType")] public string ContentType { get; set; }
        [JsonProperty("fileId")] public string FileId { get; set; }
        [JsonProperty("itemId")] public string ItemId { get; set; }
        [JsonProperty("itemType")] public string ItemType { get; set; }
        [JsonProperty("actionType")] public string ActionType { get; set; }
    }
}
