using DDDProject.Domain.Entities;
using DDDProject.Infrastructure.DbContexts;
using System.Text.RegularExpressions;

namespace DDDProject.API.Middleware
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggingMiddleware> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger, IServiceScopeFactory scopeFactory)
        {
            _next = next;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments("/api/logs", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }
            var userName = GetUserNameFromToken(context);

            var log = new Logging
            {
                RequestMethod = context.Request.Method,
                RequestPath = context.Request.Path,
                RequestBody = await ReadRequestBodyAsync(context.Request),
                Timestamp = DateTime.UtcNow,
                UserName = userName,
            };

            var originalBodyStream = context.Response.Body;
            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    try
                    {
                        _logger.LogInformation($"Request Started: {context.Request.Method} {context.Request.Path}");

                        await _next(context);

                        memoryStream.Seek(0, SeekOrigin.Begin);
                        var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                        if (!string.IsNullOrEmpty(responseBody) && responseBody.Contains("token"))
                        {
                            responseBody = Regex.Replace(responseBody, "\"token\":\"[^\"]*\"", "\"token\":\"###########\"");
                        }
                        if (!string.IsNullOrEmpty(responseBody) && responseBody.Contains("password"))
                        {
                            responseBody = Regex.Replace(responseBody, "\"password\":\"[^\"]*\"", "\"password\":\"###########\"");
                        }

                        log.ResponseBody = responseBody;

                        memoryStream.Seek(0, SeekOrigin.Begin);
                        await memoryStream.CopyToAsync(originalBodyStream);

                        log.ResponseStatusCode = context.Response.StatusCode.ToString();
                        _logger.LogInformation($"Response Sent: StatusCode {context.Response.StatusCode}");

                        dbContext.Logs.Add(log);
                        await dbContext.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An error occurred while processing the request.");
                        throw;
                    }
                }
            }
        }

        private string GetUserNameFromToken(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(token)) return null;

            try
            {
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var usernameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "unique_name");
                return usernameClaim?.Value;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            request.EnableBuffering();
            var body = await new StreamReader(request.Body).ReadToEndAsync();
            request.Body.Seek(0, SeekOrigin.Begin);

            if (!string.IsNullOrEmpty(body))
            {
                body = Regex.Replace(body, "\"password\"\\s*:\\s*\"[^\"]*\"", "\"password\":\"##########\"", RegexOptions.IgnoreCase);
            }

            return body;
        }
    }
}
