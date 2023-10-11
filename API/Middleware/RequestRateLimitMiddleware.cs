using System.Collections.Concurrent;

namespace API.Middleware;

public class RequestRateLimitMiddleware
{
    private readonly RequestDelegate _next;
    
    private readonly int _maxRequestsPerSecond;
    
    public readonly ConcurrentDictionary<string, Queue<DateTime>> requestTracker;

    public RequestRateLimitMiddleware(RequestDelegate next, int maxRequestsPerSecond)
    {
        _next = next;
        _maxRequestsPerSecond = maxRequestsPerSecond;
        requestTracker = new ConcurrentDictionary<string, Queue<DateTime>>();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string ipAddress = context.Connection.RemoteIpAddress.ToString();

        if (!CanHandleRequest(ipAddress))
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            return;
        }

        await _next(context);
    }

    public bool CanHandleRequest(string ipAddress)
    {
        DateTime currentTime = DateTime.UtcNow;

        if (!requestTracker.TryGetValue(ipAddress, out Queue<DateTime> requestTimes))
        {
            requestTimes = new Queue<DateTime>();
            requestTracker[ipAddress] = requestTimes;
        }

        requestTimes.Enqueue(currentTime);
        
        while (requestTimes.Count > 0 && (currentTime - requestTimes.Peek()).TotalSeconds > 1)
        {
            requestTimes.Dequeue();
        }

        return requestTimes.Count <= _maxRequestsPerSecond;
    }
}

public static class RequestRateLimitMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestRateLimit(this IApplicationBuilder builder, int maxRequestsPerSecond)
    {
        return builder.UseMiddleware<RequestRateLimitMiddleware>(maxRequestsPerSecond);
    }
}