using System.Globalization;
using System.Net;
using System.Text.Encodings.Web;

using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.Miscellaneous.Exceptions;

using Microsoft.Extensions.Options;

using Resend;

namespace Beatport2Rss.Api.Infrastructure.Services.EmailConfirmations;

internal sealed class ResendConfirmationEmail(
    IResend resend,
    IOptions<EmailConfirmationOptions> options)
{
    public async Task SendAsync(
        EmailConfirmation confirmation,
        string identifier,
        CancellationToken cancellationToken)
    {
        var settings = options.Value;
        var baseUri = new Uri($"{settings.PublicWebBaseUrl!.AbsoluteUri.TrimEnd('/')}/", UriKind.Absolute);
        var link = new Uri(baseUri, $"confirm-email?token={Uri.EscapeDataString(identifier)}").AbsoluteUri;
        var expiresAt = confirmation.ExpiresAt.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture);
        var hours = (confirmation.ExpiresAt - confirmation.CreatedAt).TotalHours.ToString("0.###########", CultureInfo.InvariantCulture);
        var validity = $"This link is valid for {hours} hours from issuance. This link expires at {expiresAt}.";
        var email = new EmailMessage
        {
            From = new Resend.EmailAddress
            {
                Email = settings.SenderAddress,
                DisplayName = string.IsNullOrWhiteSpace(settings.SenderName) ? null : settings.SenderName,
            },
            To = new Resend.EmailAddress { Email = confirmation.EmailAddress.Value },
            Subject = "Confirm your Beatport2Rss email address",
            TextBody = $"Confirm your Beatport2Rss email address:\n\n{link}\n\n{validity}\n\nIf you did not sign up, you can ignore this email.",
            HtmlBody = $"""
                <p><a href="{HtmlEncoder.Default.Encode(link)}">Confirm your Beatport2Rss email address</a></p>
                <p>{validity}</p>
                <p>If you did not sign up, you can ignore this email.</p>
                """,
        };

        try
        {
            await resend.EmailSendAsync($"email-confirmation/{confirmation.Id:N}", email, cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw new OperationCanceledException(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw new RetryableException("The confirmation email request timed out.");
        }
        catch (ResendException exception)
        {
            if (IsRetryable(exception))
            {
                throw new RetryableException("The email provider is temporarily unable to deliver confirmation email.");
            }

            throw new PermanentConfirmationEmailException();
        }
    }

    private static bool IsRetryable(ResendException exception) =>
        exception.StatusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests ||
        (int?)exception.StatusCode >= 500 ||
        exception.ErrorType is ErrorType.HttpSendFailed or ErrorType.ConcurrentIdempotentRequests ||
        (int?)exception.StatusCode is >= 200 and < 300;
}
