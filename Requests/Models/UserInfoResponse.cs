using System.Text.Json.Serialization;

namespace gc_bot.Requests.Models
{
    public sealed class UserInfoResponse
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; init; }

        [JsonPropertyName("reason")]
        public string? Reason { get; init; }

        // Raw JSON returned in "data" (kept as string for now)
        public string? RawDataJson { get; init; }
    }
}
