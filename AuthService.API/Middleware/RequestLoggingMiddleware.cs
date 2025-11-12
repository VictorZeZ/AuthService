using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace AuthService.API.Middleware
{
    /// <summary>
    /// Logs detailed information about each HTTP request and response in a formatted, colored box style.
    /// Enhanced for more logging to debug JWT issues.
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly IConfiguration _configuration;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger, IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var startTime = DateTime.UtcNow;

            context.Request.EnableBuffering();
            string requestBody = await ReadBodyAsync(context.Request);

            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            try
            {
                await _next(context);
                stopwatch.Stop();

                string responseBodyText = await ReadBodyAsync(context.Response);
                await LogRequestResponse(context, requestBody, responseBodyText, stopwatch.ElapsedMilliseconds, startTime);
                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                LogException(ex);  // Enhanced exception logging
                throw;
            }
        }

        private async Task<string> ReadBodyAsync(HttpRequest request)
        {
            request.Body.Position = 0;
            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            string body = await reader.ReadToEndAsync();
            request.Body.Position = 0;
            return string.IsNullOrWhiteSpace(body) ? "(empty)" : body;
        }

        private async Task<string> ReadBodyAsync(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return string.IsNullOrWhiteSpace(text) ? "(empty)" : text;
        }

        private async Task LogRequestResponse(HttpContext context, string requestBody, string responseBody, long durationMs, DateTime startTime)
        {
            var sb = new StringBuilder();

            string green = "\u001b[32m";
            string yellow = "\u001b[33m";
            string blue = "\u001b[34m";
            string magenta = "\u001b[35m";
            string cyan = "\u001b[36m";
            string red = "\u001b[31m";  // Added for errors
            string reset = "\u001b[0m";

            // Log environment and start time
            sb.AppendLine($"{cyan}Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown"}{reset}");
            sb.AppendLine($"{cyan}Start Time (UTC): {startTime}{reset}");

            sb.AppendLine($"{magenta}╔═══════════════════════════════════════════════════════════════╗{reset}");
            sb.AppendLine($"{magenta}║ {yellow}REQUEST{reset}");
            sb.AppendLine($"{magenta}╠═══════════════════════════════════════════════════════════════╝{reset}");
            sb.AppendLine($"{cyan}→ {context.Request.Method} {context.Request.Path}{reset}");
            sb.AppendLine($"{blue}Headers:{reset} {string.Join(", ", context.Request.Headers.Select(h => $"{h.Key}: {h.Value}"))}");
            sb.AppendLine($"{blue}Body:{reset} {requestBody}");
            sb.AppendLine();

            sb.AppendLine($"{magenta}╔═══════════════════════════════════════════════════════════════╗{reset}");
            sb.AppendLine($"{magenta}║ {yellow}RESPONSE{reset}");
            sb.AppendLine($"{magenta}╠═══════════════════════════════════════════════════════════════╝{reset}");
            sb.AppendLine($"{green}Status Code:{reset} {context.Response.StatusCode}");
            sb.AppendLine($"{green}Headers:{reset} {string.Join(", ", context.Response.Headers.Select(h => $"{h.Key}: {h.Value}"))}");  // Added response headers
            sb.AppendLine($"{green}Body:{reset} {responseBody}");
            sb.AppendLine($"{magenta}╚═══════════════════════════════════════════════════════════════╝{reset}");
            sb.AppendLine($"{cyan}Duration:{reset} {durationMs} ms");
            sb.AppendLine();

            // Enhanced: Log JWT config (mask secret key for security)
            var secretKey = _configuration["Jwt:SecretKey"];
            var maskedSecret = secretKey != null ? secretKey.Substring(0, 5) + "..." + secretKey.Substring(secretKey.Length - 5) : "Not found";
            sb.AppendLine($"{blue}JWT Config - SecretKey (masked):{reset} {maskedSecret}");
            sb.AppendLine($"{blue}JWT Config - Issuer:{reset} {_configuration["Jwt:Issuer"] ?? "Not found"}");
            sb.AppendLine($"{blue}JWT Config - Audience:{reset} {_configuration["Jwt:Audience"] ?? "Not found"}");

            // Enhanced: If 401, log more details for auth failure
            if (context.Response.StatusCode == 401)
            {
                sb.AppendLine($"{red}=== AUTH FAILURE DETAILS ==={reset}");
                var wwwAuth = context.Response.Headers["WWW-Authenticate"].FirstOrDefault();
                sb.AppendLine($"{red}WWW-Authenticate Header:{reset} {wwwAuth ?? "None"}");
            }

            // Enhanced: Log claims if authenticated
            var user = context.User;
            if (user.Identity?.IsAuthenticated == true)
            {
                sb.AppendLine($"{green}Authenticated User Claims:{reset}");
                foreach (var claim in user.Claims)
                {
                    sb.AppendLine($"  {claim.Type}: {claim.Value}");
                }
            }
            else
            {
                sb.AppendLine($"{red}User not authenticated.{reset}");
            }

            _logger.LogInformation(sb.ToString());
            await Task.CompletedTask;
        }

        private void LogException(Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Exception: {ex.Message}");
            sb.AppendLine($"Stack Trace: {ex.StackTrace}");
            if (ex.InnerException != null)
            {
                sb.AppendLine($"Inner Exception: {ex.InnerException.Message}");
            }
            _logger.LogError(sb.ToString());
        }
    }
}