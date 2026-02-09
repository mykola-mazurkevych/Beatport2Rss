using Beatport2Rss.Application.UseCases.Tokens.Commands;

using Mediator;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.TokenProvider;

internal interface IApplication
{
    Task RunAsync(CancellationToken cancellationToken = default);
}

internal sealed class Application(
    ILogger<Application> logger,
    IMediator mediator) :
    IApplication
{
    private static readonly Action<ILogger, string, Exception?> LogAccessToken = LoggerMessage.Define<string?>(LogLevel.Information, new EventId(), "Access token: {AccessToken}");
    private static readonly Action<ILogger, string, Exception?> LogError = LoggerMessage.Define<string?>(LogLevel.Error, new EventId(), "{ErrorMessage}");

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        var command = new RefreshBeatportAccessTokenCommand();
        var result = await mediator.Send(command, cancellationToken);

        if (result.IsSuccess)
        {
            LogAccessToken(logger, result.Value, null);

            return;
        }

        result.Errors.ToList().ForEach(error => LogError(logger, error.Message, null));
    }
}