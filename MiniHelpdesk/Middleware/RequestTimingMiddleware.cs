using System.Diagnostics;
using MiniHelpdesk.Services;

namespace MiniHelpdesk.Middleware;

public class RequestTimingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IRequestLogWriter _logWriter;

    public RequestTimingMiddleware( RequestDelegate next, IRequestLogWriter logWriter)
    {
        _next = next;
        _logWriter = logWriter;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();

        await _next(context);

        stopwatch.Stop();

        var message =
            $"HTTP {context.Request.Method} {context.Request.Path} " +
            $"responded {context.Response.StatusCode} " +
            $"in {stopwatch.ElapsedMilliseconds} ms";

        await _logWriter.WriteAsync(message);
    }
}