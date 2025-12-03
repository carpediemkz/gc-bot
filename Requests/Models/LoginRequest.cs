namespace gc_bot.Requests.Models
{
    public sealed class LoginRequest
    {
        public string Platform { get; init; } = string.Empty;
        public string Server { get; init; } = string.Empty;
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
