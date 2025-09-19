/*
 * VRChat API Documentation - WebSocket Support
 */

using Newtonsoft.Json;

namespace VRChat.API.Model.WebSocket
{
    public sealed class ModifiedImageUpdateEvent
    {
        [JsonProperty("fileId")] public string FileId { get; set; }
        [JsonProperty("pixelSize")] public int? PixelSize { get; set; }
        [JsonProperty("versionNumber")] public int? VersionNumber { get; set; }
        [JsonProperty("needsProcessing")] public bool? NeedsProcessing { get; set; }
    }
}
