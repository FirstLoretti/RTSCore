using System.Diagnostics;

using MediatR;

using Microsoft.Extensions.Logging;

namespace RTSCore.Application.Common.Behaviors;

public partial class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger
) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var timer = Stopwatch.StartNew();
        var requestName = typeof(TRequest).Name;

        LoggingExtensions.LogProcessStart(logger, requestName, request);

        var response = await next(cancellationToken);

        timer.Stop();

        LoggingExtensions.LogProcessFinish(logger, requestName, timer.ElapsedMilliseconds);

        return response;
    }
}