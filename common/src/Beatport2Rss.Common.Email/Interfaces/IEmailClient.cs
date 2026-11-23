namespace Beatport2Rss.Common.Email.Interfaces;

public interface IEmailClient
{
    Task SendAsync(
        string templateName,
        (string emailAddress, string? displayName) recipient,
        IReadOnlyDictionary<string, object> variables,
        string? idempotencyKey = null,
        CancellationToken cancellationToken = default);
}