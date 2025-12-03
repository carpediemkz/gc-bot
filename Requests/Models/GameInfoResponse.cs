namespace gc_bot.Requests.Models
{
    public sealed class GameInfoResponse
    {
        public bool Success { get; init; }
        public string? RawJson { get; init; }
        public string? Message { get; init; }
    }
}
