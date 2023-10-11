using System.Net;
using API.Middleware;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace Tests.Tests.MiddlewareTests;

public class MiddlewareTests
{
    [Theory]
    [InlineData("192.168.1.1")]
    public async Task InvokeAsync_Returns429TooManyRequests(string ipAddress)
    {
        var middleware = new RequestRateLimitMiddleware((innerContext) => Task.CompletedTask, 10);
        middleware.requestTracker.Clear();
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse(ipAddress);

        for (int i = 0; i < 10; i++)
        {
            await middleware.InvokeAsync(context);
        }
        
        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status429TooManyRequests, context.Response.StatusCode);
    }
    
    
    [Theory]
    [InlineData("192.168.1.1")]
    public async Task InvokeAsync_Returns200OK(string ipAddress)
    {
        var middleware = new RequestRateLimitMiddleware((innerContext) => Task.FromResult(0), 10);
        middleware.requestTracker.Clear();
        var context = new DefaultHttpContext();
        context.Connection.RemoteIpAddress = IPAddress.Parse(ipAddress);

        await middleware.InvokeAsync(context);

        Assert.Equal(StatusCodes.Status200OK, context.Response.StatusCode);
    }

    [Fact]
    public void CanHandleRequest_ReturnsTrue_WhenWithinLimit()
    {
        var middleware = new RequestRateLimitMiddleware((innerContext) => Task.FromResult(0), 10);
        
        var canHandle = middleware.CanHandleRequest("192.168.1.1");
        
        Assert.True(canHandle);
    }

    [Fact]
    public void CanHandleRequest_ReturnsFalse_WhenExceedsLimit()
    {
        var middleware = new RequestRateLimitMiddleware((innerContext) => Task.FromResult(0), 10);
        
        for (int i = 0; i < 15; i++)
        {
            middleware.CanHandleRequest("192.168.1.1");
        }

        var canHandle = middleware.CanHandleRequest("192.168.1.1");

        Assert.False(canHandle);
    }
}