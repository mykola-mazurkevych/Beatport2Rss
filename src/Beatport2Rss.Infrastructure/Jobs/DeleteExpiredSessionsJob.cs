using Beatport2Rss.Application.UseCases.Sessions.Commands;

using Mediator;

using Quartz;

namespace Beatport2Rss.Infrastructure.Jobs;

internal sealed class DeleteExpiredSessionsJob(IMediator mediator) :
    IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        var command = new DeleteExpiredSessionsCommand();
        await mediator.Send(command, context.CancellationToken);
    }
}