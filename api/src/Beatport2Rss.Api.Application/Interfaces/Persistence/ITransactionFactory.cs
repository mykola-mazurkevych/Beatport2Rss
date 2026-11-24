namespace Beatport2Rss.Api.Application.Interfaces.Persistence;

public interface ITransactionFactory
{
    // The unit of work commits the transaction; disposal before saving rolls it back.
    Task<IAsyncDisposable> BeginAsync(CancellationToken cancellationToken = default);
}
