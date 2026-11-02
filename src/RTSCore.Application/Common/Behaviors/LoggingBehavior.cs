using System.Diagnostics;

using MediatR;

using Microsoft.Extensions.Logging;

namespace RTSCore.Application.Common.Behaviors;

public sealed partial class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct
    )
    {
        var timer = Stopwatch.StartNew();
        var requestName = typeof(TRequest).Name;

        LoggingExtensions.LogProcessStart(logger, requestName, request);

        var response = await next(ct);

        timer.Stop();

        LoggingExtensions.LogProcessFinish(logger, requestName, timer.ElapsedMilliseconds);

        return response;
    }
}