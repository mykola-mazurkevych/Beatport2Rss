namespace Beatport2Rss.Api.Infrastructure.Services.EmailConfirmations;

internal sealed class PermanentConfirmationEmailException() :
    Exception("The email provider permanently rejected confirmation email delivery.");
