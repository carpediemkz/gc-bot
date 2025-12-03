using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using gc_bot.Requests.Models;

namespace gc_bot.Requests
{
    /// <summary>
    /// Minimal HTTP implementation of IRequestService that mirrors the provided curl request.
    /// Uses a shared HttpClient instance and attempts to parse a JSON response.
    /// </summary>
    public class ApiClient : IRequestService, IDisposable
    {
        private static readonly HttpClient _http = new HttpClient();
        private bool _disposed;

        public async Task<LoginResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));

            var url = "https://kuaiwan.com/account/quick/login/";

            using var form = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("user_name", request.Username ?? string.Empty),
                new KeyValuePair<string, string>("password", request.Password ?? string.Empty)
            });

            using var req = new HttpRequestMessage(HttpMethod.Post, url) { Content = form };

            // Mirror headers from the provided curl where appropriate
            req.Headers.Accept.ParseAdd("application/json, text/plain, */*");
            req.Headers.TryAddWithoutValidation("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7,en-AU;q=0.6");
            req.Headers.TryAddWithoutValidation("Origin", "https://kuaiwan.com");
            req.Headers.Referrer = new Uri("https://kuaiwan.com/index.html");
            req.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
            req.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
            req.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-origin");
            req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36 Edg/142.0.0.0");
            req.Headers.TryAddWithoutValidation("sec-ch-ua", "\"Chromium\";v=\"142\", \"Microsoft Edge\";v=\"142\", \"Not_A Brand\";v=\"99\"");
            req.Headers.TryAddWithoutValidation("sec-ch-ua-mobile", "?0");
            req.Headers.TryAddWithoutValidation("sec-ch-ua-platform", "\"Windows\"");

            try
            {
                using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
                var content = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!resp.IsSuccessStatusCode)
                {
                    return new LoginResponse { Success = false, Message = $"HTTP {(int)resp.StatusCode}: {content}", RawContent = UnescapeUnicodeEscapes(content) };
                }

                try
                {
                    using var doc = JsonDocument.Parse(content);
                    var root = doc.RootElement;
                    bool success = false;
                    string message = string.Empty;
                    string? token = null;


                    // New API returns { data: {...}, ok: true, reason: "" }
                    if (root.TryGetProperty("ok", out var okp) && okp.ValueKind == JsonValueKind.True)
                    {
                        success = true;
                        // prefer reason as message
                        if (root.TryGetProperty("reason", out var rp)) message = rp.GetString() ?? string.Empty;

                        if (root.TryGetProperty("data", out var dp) && dp.ValueKind == JsonValueKind.Object)
                        {
                            if (dp.TryGetProperty("online_token", out var otp)) token = otp.GetString();
                            // if message empty, try data fields
                            if (string.IsNullOrEmpty(message) && dp.TryGetProperty("user_name", out var un))
                            {
                                message = dp.GetProperty("user_name").GetString() ?? string.Empty;
                            }
                        }

        
                    }
                    else if (root.TryGetProperty("success", out var sp) && sp.ValueKind == JsonValueKind.True)
                    {
                        success = true;
                    }
                    else if (root.TryGetProperty("code", out var cp) && cp.ValueKind == JsonValueKind.Number && cp.GetInt32() == 0)
                    {
                        success = true;
                    }

                    if (string.IsNullOrEmpty(message))
                    {
                        if (root.TryGetProperty("msg", out var mp)) message = mp.GetString() ?? string.Empty;
                        else if (root.TryGetProperty("message", out var m2)) message = m2.GetString() ?? string.Empty;
                        else message = content.Length > 200 ? content[..200] : content;
                    }

                    // fallback: token at top-level
                    if (string.IsNullOrEmpty(token) && root.TryGetProperty("token", out var tp)) token = tp.GetString();

                    // Normalize escaped unicode sequences (e.g. "\\u7528\\u6237...") to readable text
                    if (!string.IsNullOrEmpty(message))
                    {
                        message = UnescapeUnicodeEscapes(message);
                    }

                    return new LoginResponse { Success = success, Message = message, Token = token, RawContent = UnescapeUnicodeEscapes(content) };
                }
                catch
                {
                    // Not JSON or parse failed: return raw content
                    return new LoginResponse { Success = true, Message = content, Token = null, RawContent = UnescapeUnicodeEscapes(content) };
                }
            }
            catch (OperationCanceledException)
            {
                return new LoginResponse { Success = false, Message = "Request cancelled" };
            }
            catch (Exception ex)
            {
                return new LoginResponse { Success = false, Message = ex.Message };
            }
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string onlineToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(onlineToken)) throw new ArgumentNullException(nameof(onlineToken));

            var url = $"https://kuaiwan.com/account/getuserinfo/?online_token={Uri.EscapeDataString(onlineToken)}";

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Accept.ParseAdd("application/json, text/plain, */*");
            req.Headers.TryAddWithoutValidation("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7,en-AU;q=0.6");
            req.Headers.TryAddWithoutValidation("Referer", "https://kuaiwan.com/index.html");
            req.Headers.TryAddWithoutValidation("Sec-Fetch-Dest", "empty");
            req.Headers.TryAddWithoutValidation("Sec-Fetch-Mode", "cors");
            req.Headers.TryAddWithoutValidation("Sec-Fetch-Site", "same-origin");
            req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36 Edg/142.0.0.0");

            try
            {
                using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
                var content = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!resp.IsSuccessStatusCode)
                {
                    return new UserInfoResponse { Ok = false, Reason = $"HTTP {(int)resp.StatusCode}", RawDataJson = UnescapeUnicodeEscapes(content) };
                }

                return new UserInfoResponse { Ok = true, Reason = string.Empty, RawDataJson = UnescapeUnicodeEscapes(content) };
            }
            catch (OperationCanceledException)
            {
                return new UserInfoResponse { Ok = false, Reason = "Request cancelled", RawDataJson = null };
            }
            catch (Exception ex)
            {
                return new UserInfoResponse { Ok = false, Reason = ex.Message, RawDataJson = null };
            }
        }

        public async Task<GameInfoResponse> GetGameInfoAsync(string onlineToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(onlineToken)) throw new ArgumentNullException(nameof(onlineToken));

            // The curl uses a server-specific host. We'll call the generic gateway path on s396 host as in the example.
            // The request requires several query parameters. If the provided onlineToken contains embedded info
            // in the form "userId|token|tp" we will use those parts; otherwise we fall back to reasonable defaults
            // and use the provided onlineToken as the ticket.

            string userId = "kw_258581991"; // fallback sample user id
            string ticket = onlineToken;
            string tp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();

            if (onlineToken.Contains('|'))
            {
                var parts = onlineToken.Split('|');
                if (parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0])) userId = parts[0];
                if (parts.Length > 1 && !string.IsNullOrWhiteSpace(parts[1])) ticket = parts[1];
                if (parts.Length > 2 && !string.IsNullOrWhiteSpace(parts[2])) tp = parts[2];
            }

            var url = $"http://s396.gcld2.teeqee.com/root/gateway.action?command=login&yx=teeqee&userId={Uri.EscapeDataString(userId)}&tp={Uri.EscapeDataString(tp)}&adult=1&ticket={Uri.EscapeDataString(ticket)}";

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            req.Headers.TryAddWithoutValidation("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7,en-AU;q=0.6");
            req.Headers.TryAddWithoutValidation("Connection", "keep-alive");
            req.Headers.TryAddWithoutValidation("Referer", "http://teeqee.com/");
            req.Headers.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
            req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36 Edg/142.0.0.0");

            // Set cookies similar to curl -b
            // Include ticket as cookie as well as REIGNID sample
            req.Headers.TryAddWithoutValidation("Cookie", $"REIGNID=E1A02DCD249A4D5DAE6DBF5C4EE53651; ticket={ticket}");

            try
            {
                using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
                var content = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!resp.IsSuccessStatusCode)
                {
                    return new GameInfoResponse { Success = false, Message = $"HTTP {(int)resp.StatusCode}", RawJson = UnescapeUnicodeEscapes(content) };
                }

                return new GameInfoResponse { Success = true, Message = string.Empty, RawJson = UnescapeUnicodeEscapes(content) };
            }
            catch (OperationCanceledException)
            {
                return new GameInfoResponse { Success = false, Message = "Request cancelled", RawJson = null };
            }
            catch (Exception ex)
            {
                return new GameInfoResponse { Success = false, Message = ex.Message, RawJson = null };
            }
        }

        public async Task<Object> GetStartGameAsync(string onlineToken, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(onlineToken)) throw new ArgumentNullException(nameof(onlineToken));

            // Build URL per provided curl example
            var url = $"http://kuaiwan.com/gameapi/page/start/?u_token={Uri.EscapeDataString(onlineToken)}&m_id=9138000&channel=undefined&s_id=396&is_exe=0";

            using var req = new HttpRequestMessage(HttpMethod.Get, url);
            req.Headers.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7");
            req.Headers.TryAddWithoutValidation("Accept-Language", "zh-CN,zh;q=0.9,en-US;q=0.8,en;q=0.7,en-AU;q=0.6");
            req.Headers.TryAddWithoutValidation("Connection", "keep-alive");
            req.Headers.TryAddWithoutValidation("Referer", "http://teeqee.com/");
            req.Headers.TryAddWithoutValidation("Upgrade-Insecure-Requests", "1");
            req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/142.0.0.0 Safari/537.36 Edg/142.0.0.0");

            try
            {
                using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseContentRead, cancellationToken).ConfigureAwait(false);
                var content = await resp.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!resp.IsSuccessStatusCode)
                {
                    return (object)$"HTTP {(int)resp.StatusCode}: {UnescapeUnicodeEscapes(content)}";
                }

                // return raw HTML/JS content
                return (object)UnescapeUnicodeEscapes(content);
            }
            catch (OperationCanceledException)
            {
                return (object)"Request cancelled";
            }
            catch (Exception ex)
            {
                return (object)ex.Message;
            }
        }



        public void Dispose()
        {
            if (_disposed) return;
            // do not dispose shared HttpClient
            _disposed = true;
        }

        private static string UnescapeUnicodeEscapes(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;

            // Replace sequences like "\u7528" with actual unicode characters
            // Use Regex to find \uXXXX patterns and convert.
            return Regex.Replace(input, "\\\\u([0-9A-Fa-f]{4})", m =>
            {
                try
                {
                    var hex = m.Groups[1].Value;
                    var code = Convert.ToInt32(hex, 16);
                    return char.ConvertFromUtf32(code);
                }
                catch
                {
                    return m.Value;
                }
            });
        }
    }
}
