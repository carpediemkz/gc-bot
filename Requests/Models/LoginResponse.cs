namespace gc_bot.Requests.Models
{
    public sealed class LoginResponse
    {
        public bool Success { get; init; }
        public string Message { get; init; } = string.Empty;
        public string? Token { get; init; }
        // Raw response body as returned by the server
        public string? RawContent { get; init; }
    }
}
