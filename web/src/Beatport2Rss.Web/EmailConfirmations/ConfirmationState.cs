namespace Beatport2Rss.Web.EmailConfirmations;

internal enum ConfirmationState
{
    Loading,
    Invalid,
    Ready,
    Submitting,
    Confirmed,
    Unavailable,
    CheckAfterSubmit
}
