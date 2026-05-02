#pragma warning disable CA1873 // Avoid potentially expensive logging

// ReSharper disable FunctionNeverReturns

using System.Globalization;

using Beatport2Rss.Application.Interfaces.Services.Misc;
using Beatport2Rss.Application.UseCases.Tokens.Commands;
using Beatport2Rss.Application.UseCases.Tokens.Queries;
using Beatport2Rss.Domain.Tokens;

using Mediator;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.TokenProvider;

internal interface IApplication
{
    Task RunAsync(
        string[] args,
        CancellationToken cancellationToken = default);
}

internal sealed class Application(
    IClock clock,
    IMediator mediator,
    ILogger<Application> logger) :
    IApplication
{
    public async Task RunAsync(
        string[] args,
        CancellationToken cancellationToken = default)
    {
        var headless = args.Contains(Constants.ArgHeadless);

        while (true)
        {
            var getUnexpiredTokenQuery = new GetUnexpiredTokenQuery();
            var getUnexpiredTokenResult = await mediator.Send(getUnexpiredTokenQuery, cancellationToken);

            if (getUnexpiredTokenResult.IsSuccess)
            {
                var token = getUnexpiredTokenResult.Value;

                Log.AccessToken(
                    logger,
                    token.AccessToken,
                    token.ExpiresAt.ToLocalTime().ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture));

                var delay = (token.ExpiresAt - clock.UtcNow).Add(TimeSpan.FromMinutes(-1));
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, cancellationToken);
                }
            }

            Log.Refreshing(logger);

            var refreshCommand = new RefreshBeatportAccessTokenCommand(headless);
            var refreshResult = await mediator.Send(refreshCommand, cancellationToken);

            Log.IsSuccess(logger, refreshResult.IsSuccess);

            if (!refreshResult.IsSuccess)
            {
                refreshResult.Errors.ToList().ForEach(error => Log.Error(logger, error.Message));
            }
        }
    }
}

internal static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Refreshing...")]
    public static partial void Refreshing(ILogger logger);

    [LoggerMessage(Level = LogLevel.Information, Message = "Success: {IsSuccess}")]
    public static partial void IsSuccess(ILogger logger, bool isSuccess);

    [LoggerMessage(Level = LogLevel.Error, Message = "{ErrorMessage}")]
    public static partial void Error(ILogger logger, string errorMessage);

    [LoggerMessage(Level = LogLevel.Information, Message = "Access token: {BeatportAccessToken}\n\tExpires at: {ExpiresAt}")]
    public static partial void AccessToken(ILogger logger, BeatportAccessToken beatportAccessToken, string expiresAt);
}