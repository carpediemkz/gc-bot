using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace gc_bot.Requests
{
    public sealed class ParsedLogin
    {
        public string? RawJson { get; init; }
        public string? OnlineToken { get; init; }
        public string? UserName { get; init; }
        public string? UserId { get; init; }
        public Dictionary<string, string> Cookies { get; init; } = new(StringComparer.OrdinalIgnoreCase);

        public override string ToString()
        {
            var parts = new List<string>();
            if (!string.IsNullOrEmpty(UserName)) parts.Add($"UserName={UserName}");
            if (!string.IsNullOrEmpty(UserId)) parts.Add($"UserId={UserId}");
            if (!string.IsNullOrEmpty(OnlineToken)) parts.Add($"OnlineToken={OnlineToken}");

            if (Cookies != null && Cookies.Count > 0)
            {
                var cookieStr = string.Join("; ", Cookies.Select(kv => $"{kv.Key}={kv.Value}"));
                parts.Add($"Cookies=[{cookieStr}]");
            }

            //if (!string.IsNullOrEmpty(RawJson))
            //{
            //    var preview = RawJson.Length > 200 ? RawJson.Substring(0, 200) + "..." : RawJson;
            //    parts.Add($"RawJson={preview}");
            //}

            return string.Join(",\n", parts);
        }
    }

    public static class ParsedLoginParser
    {
        public static ParsedLogin ParseLoginAndCookies(string raw)
        {
            var result = new ParsedLogin();
            if (string.IsNullOrWhiteSpace(raw)) return result;

            const string splitToken = "\n\nCookies:";
            var idx = raw.IndexOf(splitToken, StringComparison.Ordinal);
            var jsonPart = idx >= 0 ? raw.Substring(0, idx).Trim() : raw.Trim();
            var cookiesPart = idx >= 0 ? raw.Substring(idx + splitToken.Length).Trim() : string.Empty;

            // Extract JSON object from jsonPart
            var firstBrace = jsonPart.IndexOf('{');
            var lastBrace = jsonPart.LastIndexOf('}');
            var jsonText = (firstBrace >= 0 && lastBrace > firstBrace) ? jsonPart[firstBrace..(lastBrace + 1)] : jsonPart;

            string? onlineToken = null;
            string? userName = null;
            string? userId = null;

            try
            {
                using var doc = JsonDocument.Parse(jsonText);
                var root = doc.RootElement;

                if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object)
                {
                    if (data.TryGetProperty("online_token", out var t) && t.ValueKind == JsonValueKind.String)
                        onlineToken = t.GetString();

                    if (data.TryGetProperty("user_name", out var un) && un.ValueKind == JsonValueKind.String)
                        userName = un.GetString();

                    if (data.TryGetProperty("user_id", out var uid) && uid.ValueKind != JsonValueKind.Undefined)
                        userId = uid.ToString();
                }
                else
                {
                    if (root.TryGetProperty("token", out var topToken) && topToken.ValueKind == JsonValueKind.String)
                        onlineToken = topToken.GetString();
                }
            }
            catch
            {
                // ignore parse errors
            }

            var cookies = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrWhiteSpace(cookiesPart))
            {
                foreach (var pair in cookiesPart.Split(';'))
                {
                    var kv = pair.Split('=', 2);
                    if (kv.Length == 2)
                    {
                        var k = kv[0].Trim();
                        var v = kv[1].Trim();
                        if (!string.IsNullOrEmpty(k)) cookies[k] = v;
                    }
                }
            }

            return new ParsedLogin
            {
                RawJson = jsonText,
                OnlineToken = onlineToken,
                UserName = userName,
                UserId = userId,
                Cookies = cookies
            };
        }
    }
}
