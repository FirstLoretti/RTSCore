using System.Diagnostics;

using MediatR;

namespace RTSCore.Application.Common.Behaviors;

public class LoggingBehavior<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken
    )
    {
        var timer = Stopwatch.StartNew();

        Console.WriteLine($"[MEDIATR-START] Запущен процесс {typeof(TRequest).Name}");

        var response = await next(cancellationToken);
        timer.Stop();

        Console.WriteLine(
            $"[MEDIATR-FINISH] Процесс {typeof(TRequest).Name} завершился за {timer.ElapsedMilliseconds}"
        );

        return response;
    }
}