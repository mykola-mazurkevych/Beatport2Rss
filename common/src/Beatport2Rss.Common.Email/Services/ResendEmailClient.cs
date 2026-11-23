using Beatport2Rss.Common.Email.Interfaces;

using Resend;

namespace Beatport2Rss.Common.Email.Services;

internal sealed class ResendEmailClient(IResend resend) :
    IEmailClient
{
    public Task SendAsync(
        string templateId,
        (string emailAddress, string? displayName) recipient,
        IReadOnlyDictionary<string, object> variables,
        string? idempotencyKey = null,
        CancellationToken cancellationToken = default)
    {
        var emailMessage = new EmailMessage
        {
            To = new EmailAddress
            {
                Email = recipient.emailAddress,
                DisplayName = recipient.displayName,
            },
            Template = new EmailMessageTemplate
            {
                TemplateId = templateId,
                Variables = variables.ToDictionary(),
            }
        };

        return string.IsNullOrEmpty(idempotencyKey)
            ? resend.EmailSendAsync(emailMessage, cancellationToken)
            : resend.EmailSendAsync(idempotencyKey, emailMessage, cancellationToken);
    }
}