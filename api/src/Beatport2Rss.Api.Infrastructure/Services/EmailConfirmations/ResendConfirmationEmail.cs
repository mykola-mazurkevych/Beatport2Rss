using System.Globalization;
using System.Net;
using System.Text.Encodings.Web;

using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
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
        EmailVerification verification,
        string identifier,
        CancellationToken cancellationToken)
    {
        var settings = options.Value;
        var baseUri = new Uri($"{settings.PublicWebBaseUrl.TrimEnd('/')}/", UriKind.Absolute);
        var link = new Uri(baseUri, $"confirm-email?token={Uri.EscapeDataString(identifier)}").AbsoluteUri;
        var expiresAt = verification.ExpiresAt.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture);
        var email = new EmailMessage
        {
            From = new Resend.EmailAddress
            {
                Email = settings.SenderAddress,
                DisplayName = string.IsNullOrWhiteSpace(settings.SenderName) ? null : settings.SenderName,
            },
            To = new Resend.EmailAddress { Email = verification.EmailAddress.Value },
            Subject = "Confirm your Beatport2Rss email address",
            TextBody = $"Confirm your Beatport2Rss email address:\n\n{link}\n\nThis link expires at {expiresAt}.\n\nIf you did not sign up, you can ignore this email.",
            HtmlBody = $"""
                <p><a href="{HtmlEncoder.Default.Encode(link)}">Confirm your Beatport2Rss email address</a></p>
                <p>This link expires at {expiresAt}.</p>
                <p>If you did not sign up, you can ignore this email.</p>
                """,
        };

        try
        {
            await resend.EmailSendAsync($"email-confirmation/{verification.Id:N}", email, cancellationToken);
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
