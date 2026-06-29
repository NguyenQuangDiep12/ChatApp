using ChatApp.Core.Services;

namespace ChatApp.API.Middlewares
{
    /// <summary>
    /// Middleware that validates JWT tokens against the database session store.
    /// This ensures that logged-out or revoked tokens are rejected even if still cryptographically valid.
    /// </summary>
    public class TokenRevocationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenRevocationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ISessionService sessionService)
        {
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader["Bearer ".Length..].Trim();

                if (!string.IsNullOrEmpty(token))
                {
                    var isValid = await sessionService.IsTokenValidAsync(token);
                    if (!isValid)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync("{\"message\":\"Token has been revoked or expired\"}");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
