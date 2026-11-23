namespace Beatport2Rss.Api.Application.UseCases.EmailConfirmations;

public sealed class PermanentConfirmationEmailException : Exception
{
    public PermanentConfirmationEmailException() :
        base("The email provider permanently rejected confirmation email delivery.")
    {
    }

    public PermanentConfirmationEmailException(string message) :
        base(message)
    {
    }

    public PermanentConfirmationEmailException(string message, Exception innerException) :
        base(message, innerException)
    {
    }
}
