using Beatport2Rss.Api.Application.UseCases.EmailVerifications;
using Beatport2Rss.Api.Infrastructure.Persistence;

using Microsoft.Extensions.Options;

using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace Beatport2Rss.Api.Infrastructure.Services.EmailVerifications;

internal sealed class WolverineConfirmationEmailOutbox(
    IDbContextOutbox<ApiDbContext> outbox,
    IOptions<EmailConfirmationOptions> options) :
    IConfirmationEmailOutbox
{
    public ValueTask EnqueueAsync(string identifier) =>
        outbox.PublishAsync(new SendConfirmationEmail(identifier), new DeliveryOptions { DeliverWithin = options.Value.Lifetime });
}
