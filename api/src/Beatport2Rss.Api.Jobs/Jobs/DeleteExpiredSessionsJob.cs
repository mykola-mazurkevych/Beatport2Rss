using Beatport2Rss.Api.Application.UseCases.Sessions.Commands;

using Mediator;

using Quartz;

namespace Beatport2Rss.Api.Jobs.Jobs;

internal sealed class DeleteExpiredSessionsJob(IMediator mediator) :
    IJob
{
    public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
    {
        var command = new DeleteExpiredSessionsCommand();
        await mediator.Send(command, cancellationToken);
    }
}