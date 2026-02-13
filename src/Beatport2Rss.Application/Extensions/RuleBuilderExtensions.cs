using System.Net;

using Beatport2Rss.Domain.Feeds;
using Beatport2Rss.Domain.Sessions;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.Domain.Users;

using FluentValidation;

namespace Beatport2Rss.Application.Extensions;

internal static class RuleBuilderExtensions
{
    extension<T>(IRuleBuilderInitial<T, string?> ruleBuilder)
    {
        public void IsEmailAddress() =>
            ruleBuilder
                .Cascade(CascadeMode.Stop)
                .IsNotEmpty("Email address is required.")
                .IsNotTooLong(EmailAddress.MaxLength, "Email address must be at most {MaxLength} characters.")
                .EmailAddress().WithMessage("A valid email address is required.");

        public void IsFeedName() =>
            ruleBuilder
                .IsNotEmpty("Feed name is required.")
                .IsNotTooLong(FeedName.MaxLength, "Feed name must be at most {MaxLength} characters.");

        public void IsFirstName() =>
            ruleBuilder
                .IsNotTooLong(User.NameLength, "First name must be at most {MaxLength} characters long.");

        public void IsIpAddress() =>
            ruleBuilder
                .IsNotTooLong(Session.IpAddressMaxLength, "IP address must be at most {MaxLength} characters long.")
                .Must(s => IPAddress.TryParse(s, out _)).WithMessage("IP address is not valid.");

        public void IsLastName() =>
            ruleBuilder
                .IsNotTooLong(User.NameLength, "Last name must be at most {MaxLength} characters long.");

        public void IsPassword() =>
            ruleBuilder
                .Cascade(CascadeMode.Stop)
                .IsNotEmpty("Password is required.")
                .IsNotTooShort(Password.MinLength, "Password must be at least {MinLength} characters long.")
                .IsNotTooLong(Password.MaxLength, "Password must be at most {MaxLength} characters.");

        public void IsRefreshToken() =>
            ruleBuilder
                .Cascade(CascadeMode.Stop)
                .IsNotEmpty("Refresh token is required.")
                .IsExactlyLong(RefreshToken.Length, "Refresh token must be exactly {TotalLength} characters long.");

        public void IsTagName() =>
            ruleBuilder
                .IsNotEmpty("Tag name is required.")
                .IsNotTooLong(TagName.MaxLength, "Tag name must be at most {MaxLength} characters.");

        public void IsUserAgent() =>
            ruleBuilder
                .IsNotTooLong(Session.UserAgentMaxLength, "User agent must be at most {MaxLength} characters long.");
    }

    extension<T>(IRuleBuilder<T, string?> ruleBuilder)
    {
        private void IsExactlyLong(int length, string message) =>
            ruleBuilder.Length(length).WithMessage(message);

        private IRuleBuilder<T, string?> IsNotEmpty(string message) =>
            ruleBuilder.NotEmpty().WithMessage(message);

        private IRuleBuilder<T, string?> IsNotTooLong(int maximumLength, string message) =>
            ruleBuilder.MaximumLength(maximumLength).WithMessage(message);

        private IRuleBuilder<T, string?> IsNotTooShort(int minimumLength, string message) =>
            ruleBuilder.MinimumLength(minimumLength).WithMessage(message).WithErrorCode("SOME-CODE");
    }
}