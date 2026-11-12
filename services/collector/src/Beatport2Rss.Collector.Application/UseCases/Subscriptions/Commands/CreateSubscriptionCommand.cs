using Beatport2Rss.Collector.Application.Exceptions;
using Beatport2Rss.Collector.Application.Interfaces.Services;
using Beatport2Rss.Common.Domain.Beatport;
using Beatport2Rss.Common.Domain.Subscriptions;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.FluentResults.Errors;
using Beatport2Rss.Common.FluentResults.Extensions;

using Microsoft.Extensions.Logging;

namespace Beatport2Rss.Collector.Application.UseCases.Subscriptions.Commands;

internal sealed record CreateSubscriptionCommand(
    SubscriptionType Type,
    int BeatportId);

internal sealed class CreateSubscriptionCommandHandler(
    ISubscriptionService subscriptionService,
    IUnitOfWork unitOfWork,
    ILogger<CreateSubscriptionCommand> logger)
{
    public async Task HandleAsync(CreateSubscriptionCommand command, CancellationToken cancellationToken)
    {
        CreateSubscriptionCommandLoggerMessages.Creating(logger, command.Type, command.BeatportId);

        var beatportId = BeatportId.Create(command.BeatportId);
        var subscriptionResult = await subscriptionService.EnsureSubscriptionExistsAsync(command.Type, beatportId, cancellationToken);
        if (subscriptionResult.IsFailed)
        {
            CreateSubscriptionCommandLoggerMessages.BeatportFailure(logger, subscriptionResult.ErrorMessage);

            if (subscriptionResult.Errors.Any(error => error is TransientError or TransientHttpError or RequestTimeoutError or TooManyRequestsError))
            {
                throw new RetryableBeatportRequestException(subscriptionResult.ErrorMessage);
            }

            return;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}

internal static partial class CreateSubscriptionCommandLoggerMessages
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Creating {SubscriptionType} subscription {BeatportId}")]
    public static partial void Creating(ILogger logger, SubscriptionType subscriptionType, int beatportId);

    [LoggerMessage(Level = LogLevel.Warning, Message = "Failed to get subscription details from Beatport: {ErrorMessage}")]
    public static partial void BeatportFailure(ILogger logger, string errorMessage);
}