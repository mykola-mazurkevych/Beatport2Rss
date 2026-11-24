using System.Globalization;
using System.Net;

using Beatport2Rss.Api.Application.UseCases.EmailConfirmations;
using Beatport2Rss.Api.Domain.EmailConfirmations;
using Beatport2Rss.Common.Email.Interfaces;
using Beatport2Rss.Common.Miscellaneous.Exceptions;

using Microsoft.Extensions.Options;

using Resend;

using DeliveryConfirmationOptions = Beatport2Rss.Api.Infrastructure.Options.EmailConfirmationOptions;

namespace Beatport2Rss.Api.Infrastructure.Services.EmailConfirmations;

internal sealed class ResendConfirmationEmail(
    IEmailClient emailClient,
    IOptions<DeliveryConfirmationOptions> options) : IConfirmationEmailSender
{
    public async Task SendAsync(
        EmailConfirmation confirmation,
        CancellationToken cancellationToken)
    {
        var settings = options.Value;
        var baseUri = new Uri($"{settings.PublicWebBaseUrl!.AbsoluteUri.TrimEnd('/')}/", UriKind.Absolute);
        var link = new Uri(baseUri, $"confirm-email?token={confirmation.Id}").AbsoluteUri;
        var expiresAt = confirmation.ExpiresAt.ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss 'UTC'", CultureInfo.InvariantCulture);
        var hours = (confirmation.ExpiresAt - confirmation.CreatedAt).TotalHours.ToString("0.###########", CultureInfo.InvariantCulture);
        var variables = new Dictionary<string, object>
        {
            ["CONFIRMATION_URL"] = link,
            ["EXPIRES_AT"] = expiresAt,
            ["VALID_HOURS"] = hours,
        };

        try
        {
            await emailClient.SendAsync(
                settings.TemplateId,
                (confirmation.EmailAddress.Value, null),
                variables,
                $"email-confirmation/{confirmation.Id:N}",
                cancellationToken);
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
