using System.Text;

namespace TeachMate.Api;

public class SwaggerBasicAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public SwaggerBasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task Invoke(HttpContext context)
    {
        // Only protect Swagger in non-development environments
        if (context.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase))
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var encodedCredentials = authHeader["Basic ".Length..].Trim();
                    var decodedBytes = Convert.FromBase64String(encodedCredentials);
                    var decodedString = Encoding.UTF8.GetString(decodedBytes);
                    var credentials = decodedString.Split(':', 2);

                    var expectedUsername = _configuration["SwaggerAuth:Username"];
                    var expectedPassword = _configuration["SwaggerAuth:Password"];

                    if (credentials.Length == 2 &&
                        credentials[0] == expectedUsername &&
                        credentials[1] == expectedPassword)
                    {
                        await _next(context);
                        return;
                    }
                }
                catch
                {
                    // Ignore and fall through to Unauthorized response
                }
            }

            context.Response.Headers.WWWAuthenticate = "Basic";
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized");
            return;
        }

        await _next(context);
    }
}

