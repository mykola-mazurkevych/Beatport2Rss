using Beatport2Rss.Api.Application.Interfaces.Messages;
using Beatport2Rss.Api.Application.Interfaces.Persistence.Repositories;
using Beatport2Rss.Api.Application.Interfaces.Services.Messaging;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.EntityFrameworkCore.Persistence.Interfaces;
using Beatport2Rss.Common.IntegrationEvents.V1.Tags;
using Beatport2Rss.Common.Miscellaneous.Interfaces;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

using FluentResults;

using Mediator;

namespace Beatport2Rss.Api.Application.UseCases.Tags.Commands;

public sealed record DeleteTagCommand(
    UserId UserId,
    Slug TagSlug) :
    ICommand<Result>, IRequireUser, IRequireTag;

internal sealed class DeleteTagCommandHandler(
    IClock clock,
    ITagCommandRepository tagCommandRepository,
    IIntegrationEventOutbox integrationEventOutbox,
    IUnitOfWork unitOfWork) :
    ICommandHandler<DeleteTagCommand, Result>
{
    public async ValueTask<Result> Handle(
        DeleteTagCommand command,
        CancellationToken cancellationToken)
    {
        var tag = await tagCommandRepository.LoadAsync(command.UserId, command.TagSlug, cancellationToken);
        tagCommandRepository.Delete(tag);

        var tagDeleted = new TagDeletedV1(
            EventId: Guid.CreateVersion7(),
            OccurredAt: clock.UtcNow,
            tag.Id.Value);
        await integrationEventOutbox.EnqueueAsync(tagDeleted, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok();
    }
}