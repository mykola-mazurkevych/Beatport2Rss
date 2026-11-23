using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Infrastructure.Persistence;

using Microsoft.Extensions.Options;

using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace Beatport2Rss.Api.Infrastructure.Services.EmailConfirmations;

internal sealed class WolverineConfirmationEmailOutbox(
    IDbContextOutbox<ApiDbContext> outbox,
    IOptions<EmailConfirmationOptions> options) :
    IConfirmationEmailOutbox
{
    public ValueTask EnqueueAsync(EmailConfirmationId id) =>
        outbox.PublishAsync(new SendConfirmationEmail(id.ToString()), new DeliveryOptions { DeliverWithin = options.Value.Lifetime });
}
