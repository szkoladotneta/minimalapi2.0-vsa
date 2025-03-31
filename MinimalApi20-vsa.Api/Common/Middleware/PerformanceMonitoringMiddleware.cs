using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MinimalApi20_vsa.Api.Common.Middleware;

public class PerformanceMonitoringMiddleware(RequestDelegate next, ILogger<PerformanceMonitoringMiddleware> logger)
{
    private readonly TimeSpan _threshold = TimeSpan.FromSeconds(1);

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            await next(context);
        }
        finally
        {
            stopwatch.Stop();
            var elapsed = stopwatch.Elapsed;
            
            if (elapsed > _threshold)
            {
                logger.LogWarning(
                    "Request {RequestPath} took {ElapsedMilliseconds}ms which exceeds the threshold of {ThresholdMilliseconds}ms",
                    context.Request.Path,
                    elapsed.TotalMilliseconds,
                    _threshold.TotalMilliseconds);
            }
        }
    }
}

public static class PerformanceMonitoringMiddlewareExtensions
{
    public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<PerformanceMonitoringMiddleware>();
    }
}
