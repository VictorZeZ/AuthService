using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace AuthService.API.Middleware
{
    /// <summary>
    /// Request / Response logging middleware.
    /// - Uses buffering with a configurable maximum body size to avoid memory DoS.
    /// - Masks sensitive headers (Authorization, Cookie).
    /// - Uses structured logging for main summary and Debug for full bodies.
    /// - Restores response body stream in finally block.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        // Max bytes to read from request/response body to avoid large memory usage.
        // You may want to move this to options/config later.
        private const int DefaultMaxBodyLength = 64 * 1024; // 64 KB

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IWebHostEnvironment env)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _env = env ?? throw new ArgumentNullException(nameof(env));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var startTime = DateTime.UtcNow;

            // Enable buffering so we can read the request body multiple times.
            // Keep a reasonable limit when reading the body.
            context.Request.EnableBuffering();

            string requestBody = await ReadStreamAsStringAsync(context.Request.Body, DefaultMaxBodyLength, context.Request.ContentType);
            // make sure to rewind for downstream middleware/controllers
            context.Request.Body.Position = 0;

            var originalResponseBody = context.Response.Body;
            await using var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            try
            {
                await _next(context);
                stopwatch.Stop();

                // Read response body (seek to beginning)
                context.Response.Body.Seek(0, SeekOrigin.Begin);
                string responseBody = await ReadStreamAsStringAsync(context.Response.Body, DefaultMaxBodyLength, context.Response.ContentType);
                context.Response.Body.Seek(0, SeekOrigin.Begin);

                // Structured summary log (Information)
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {ElapsedMilliseconds}ms",
                    context.Request.Method,
                    context.Request.Path + context.Request.QueryString,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);

                // Detailed debug log (bodies, headers, claims) only at Debug level
                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    var details = new
                    {
                        Environment = _env.EnvironmentName,
                        StartTimeUtc = startTime,
                        Request = new
                        {
                            Method = context.Request.Method,
                            Path = context.Request.Path + context.Request.QueryString,
                            Headers = MaskHeaders(context.Request.Headers),
                            Body = TruncateForLog(requestBody, DefaultMaxBodyLength)
                        },
                        Response = new
                        {
                            StatusCode = context.Response.StatusCode,
                            Headers = MaskHeaders(context.Response.Headers),
                            Body = TruncateForLog(responseBody, DefaultMaxBodyLength)
                        },
                        DurationMs = stopwatch.ElapsedMilliseconds,
                        Authenticated = context.User?.Identity?.IsAuthenticated == true,
                        Claims = context.User?.Identity?.IsAuthenticated == true
                            ? context.User.Claims.Select(c => new { c.Type, c.Value }).ToArray()
                            : Array.Empty<object>()
                    };

                    _logger.LogDebug("HTTP details: {@Details}", details);
                }

                // If status code indicates auth failure, log limited diagnostics at Warning level
                if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
                {
                    _logger.LogWarning("Auth failure for {Path}. StatusCode: {StatusCode}. WWW-Authenticate: {Www}",
                        context.Request.Path,
                        context.Response.StatusCode,
                        context.Response.Headers["WWW-Authenticate"].FirstOrDefault() ?? "(none)");
                }

                // copy back the response body to the original stream
                await responseBodyStream.CopyToAsync(originalResponseBody);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                // Log exception with structured fields.
                _logger.LogError(ex, "Unhandled exception while processing request {Method} {Path}", context.Request.Method, context.Request.Path);
                throw;
            }
            finally
            {
                // Ensure the original response body stream is restored (in case of error)
                context.Response.Body = originalResponseBody;
            }
        }

        /// <summary>
        /// Safely reads up to maxBytes from a stream as UTF8 text. Returns "(binary or too large)" if not text or truncated.
        /// </summary>
        private static async Task<string> ReadStreamAsStringAsync(Stream stream, int maxBytes, string? contentType)
        {
            // If stream cannot seek, try to copy into a MemoryStream first (but respect maxBytes).
            if (!stream.CanSeek)
            {
                using var buffer = new MemoryStream();
                await stream.CopyToAsync(buffer);
                buffer.Seek(0, SeekOrigin.Begin);
                return ReadFromMemoryStream(buffer, maxBytes, contentType);
            }

            return ReadFromMemoryStream(stream, maxBytes, contentType);
        }

        private static string ReadFromMemoryStream(Stream stream, int maxBytes, string? contentType)
        {
            try
            {
                // Heuristic: if contentType is likely binary (image, audio, video, application/octet-stream) we avoid trying to decode
                if (!string.IsNullOrEmpty(contentType))
                {
                    var ct = contentType.ToLowerInvariant();
                    if (ct.Contains("image") || ct.Contains("audio") || ct.Contains("video") || ct.Contains("octet-stream") || ct.Contains("pdf"))
                    {
                        return "(binary content)";
                    }
                }

                // read up to maxBytes
                stream.Seek(0, SeekOrigin.Begin);
                var toRead = (int)Math.Min(maxBytes, stream.Length);
                var buffer = new byte[toRead];
                var read = stream.Read(buffer, 0, toRead);

                // If the stream is longer than maxBytes, indicate truncation
                bool truncated = stream.Length > maxBytes;

                // Try decode as UTF8; if fails, return binary marker
                try
                {
                    var text = Encoding.UTF8.GetString(buffer, 0, read);
                    if (truncated) text = text + "...(truncated)";
                    if (string.IsNullOrWhiteSpace(text)) return "(empty)";
                    return text;
                }
                catch
                {
                    return "(binary content)";
                }
            }
            catch
            {
                return "(unreadable)";
            }
        }

        /// <summary>
        /// Mask sensitive headers like Authorization and Cookie.
        /// Returns dictionary-like object for logging.
        /// </summary>
        private static IDictionary<string, string> MaskHeaders(IHeaderDictionary headers)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var h in headers)
            {
                var key = h.Key;
                var val = string.Join(", ", h.Value.ToArray());

                if (string.Equals(key, "Authorization", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrEmpty(val))
                    {
                        // keep only scheme (e.g. "Bearer") and mask the token
                        var parts = val.Split(' ', 2);
                        if (parts.Length == 2)
                        {
                            var scheme = parts[0];
                            result[key] = $"{scheme} [REDACTED]";
                        }
                        else
                        {
                            result[key] = "[REDACTED]";
                        }
                    }
                    else
                    {
                        result[key] = "(empty)";
                    }
                }
                else if (string.Equals(key, "Cookie", StringComparison.OrdinalIgnoreCase))
                {
                    result[key] = "[REDACTED]";
                }
                else
                {
                    // For ordinary headers we include the value; consider truncating very large headers
                    var safeVal = val.Length > 4096 ? val.Substring(0, 4096) + "...(truncated)" : val;
                    result[key] = safeVal;
                }
            }

            return result;
        }

        private static string TruncateForLog(string value, int max)
        {
            if (string.IsNullOrEmpty(value)) return value ?? "(empty)";
            return value.Length <= max ? value : value.Substring(0, max) + "...(truncated)";
        }
    }

    // Extension to wire up middleware cleanly in Program.cs
    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}