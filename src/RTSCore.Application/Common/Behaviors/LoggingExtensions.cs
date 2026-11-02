using Microsoft.Extensions.Logging;

namespace RTSCore.Application.Common.Behaviors;

public static partial class LoggingExtensions
{
    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Started processing: {RequestName} with parameters: {Request}")]
    public static partial void LogProcessStart(ILogger logger, string requestName, object request);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Finished processing: {RequestName} in {ElapsedMs} ms")]
    public static partial void LogProcessFinish(ILogger logger, string requestName, long elapsedMs);
}