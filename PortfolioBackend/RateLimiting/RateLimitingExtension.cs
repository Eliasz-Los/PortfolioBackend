using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

namespace PortfolioBackend.RateLimiting;

public static class RateLimitingExtension
{
    public const string GlobalPolicy = "global";
    public const string PathfindingPolicy = "pathfinding";

    public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.OnRejected = static async (context, token) =>
            {
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    var seconds = Math.Ceiling(retryAfter.TotalSeconds);
                    context.HttpContext.Response.Headers.RetryAfter = seconds.ToString(CultureInfo.InvariantCulture);
                }

                await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
            };

            options.AddPolicy(GlobalPolicy, httpContext =>
            {
                var key = GetClientKey(httpContext);
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: key,
                    factory: static _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 120,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    });
            });

            options.AddPolicy(PathfindingPolicy, httpContext =>
            {
                var key = GetClientKey(httpContext);
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: key,
                    factory: static _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        AutoReplenishment = true
                    });
            });
        });
        return services;
    }
    
    private static string GetClientKey(HttpContext context)
    {
        
        // If you're behind a reverse proxy, these headers may be set.
        // IMPORTANT: only trust them if you also enable ForwardedHeaders middleware.
        string? forwardedFor = context.Request.Headers["X-Forwarded-For"].ToString();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            // Can be a comma-separated list. Take the first entry.
            var first = forwardedFor.Split(',')[0].Trim();
            if (IPAddress.TryParse(first, out _))
            {
                return $"ip:{first}";
            }
        }

        
        string? realIp = context.Request.Headers["X-Real-IP"].ToString();
        if (!string.IsNullOrWhiteSpace(realIp) && IPAddress.TryParse(realIp.Trim(), out _))
        {
            return $"ip:{realIp.Trim()}";
        }
        
        var remoteIp = context.Connection.RemoteIpAddress;
        if (remoteIp != null)
        {
            return $"ip:{remoteIp}";
        }
        
        // Last resort: stable-ish key (avoid per-request randomness)
        var ua = context.Request.Headers.UserAgent.ToString();
        if (!string.IsNullOrWhiteSpace(ua))
        {
            return $"ua:{ua}";
        }
        
        return "unknown";
    }
}