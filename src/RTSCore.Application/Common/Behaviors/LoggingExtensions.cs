using Microsoft.Extensions.Logging;

namespace RTSCore.Application.Common.Behaviors;

public static partial class LoggingExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Запущен процесс {RequestName} с параметрами: {Request}")]
    public static partial void LogProcessStart(ILogger logger, string requestName, object request);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Процесс {RequestName} завершился за {ElapsedMs} мс")]
    public static partial void LogProcessFinish(ILogger logger, string requestName, long elapsedMs);
}