using Beatport2Rss.Api.Application.UseCases.Users.Commands;
using Beatport2Rss.Common.IntegrationEvents.EmailConfirmations;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations.IntegrationEvents;

public static class EmailConfirmedHandler
{
    public static async Task HandleAsync(
        EmailConfirmed message,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(
            new UpdateUserStatusCommand(message.UserId, IsActive: true, message.EmailAddress),
            cancellationToken);
        if (result.IsFailed)
        {
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(error => error.Message)));
        }
    }
}
