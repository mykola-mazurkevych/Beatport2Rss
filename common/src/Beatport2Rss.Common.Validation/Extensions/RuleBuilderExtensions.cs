#pragma warning disable CA1034 // Nested types should not be visible

using System.Net;

using Beatport2Rss.Common.SharedKernel.Constants;

using FluentValidation;

namespace Beatport2Rss.Common.Validation.Extensions;

public static class RuleBuilderExtensions
{
    extension<T>(IRuleBuilderInitial<T, int> ruleBuilder)
    {
        public void IsPositive() =>
            ruleBuilder
                .GreaterThan(0)
                .WithMessage("'{PropertyName}' must be a positive value.");
    }

    extension<T>(IRuleBuilderInitial<T, string?> ruleBuilder)
    {
        public void IsCountryCode() =>
            ruleBuilder
                .IsNotTooLong(MaxLengthConstants.CountryCode, "Country code must be ad most {MaxLength} characters long.");

        public void IsEmailAddress() =>
            ruleBuilder
                .Cascade(CascadeMode.Stop)
                .IsNotEmpty("Email address is required.")
                .IsNotTooLong(MaxLengthConstants.EmailAddress, "Email address must be at most {MaxLength} characters long.")
                .EmailAddress().WithMessage("A valid email address is required.");

        public void IsFeedName() =>
            ruleBuilder
                .IsNotEmpty("Feed name is required.")
                .IsNotTooLong(MaxLengthConstants.FeedName, "Feed name must be at most {MaxLength} characters long.");

        public void IsFirstName() =>
            ruleBuilder
                .IsNotTooLong(MaxLengthConstants.Name, "First name must be at most {MaxLength} characters long.");

        public void IsIpAddress() =>
            ruleBuilder
                .IsNotTooLong(MaxLengthConstants.IpAddress, "IP address must be at most {MaxLength} characters long.")
                .Must(s => IPAddress.TryParse(s, out _)).WithMessage("IP address is not valid.");

        public void IsLastName() =>
            ruleBuilder
                .IsNotTooLong(MaxLengthConstants.Name, "Last name must be at most {MaxLength} characters long.");

        public void IsPassword() =>
            ruleBuilder
                .Cascade(CascadeMode.Stop)
                .IsNotEmpty("Password is required.")
                .IsNotTooShort(MinLengthConstants.Password, "Password must be at least {MinLength} characters long.")
                .IsNotTooLong(MaxLengthConstants.Password, "Password must be at most {MaxLength} characters long.");

        public void IsRefreshToken() =>
            ruleBuilder
                .Cascade(CascadeMode.Stop)
                .IsNotEmpty("Refresh token is required.")
                .IsExactlyLong(ExactLengthConstants.RefreshToken, "Refresh token must be exactly {TotalLength} characters long.");

        public void IsTagName() =>
            ruleBuilder
                .IsNotEmpty("Tag name is required.")
                .IsNotTooLong(MaxLengthConstants.TagName, "Tag name must be at most {MaxLength} characters long.");

        public void IsUserAgent() =>
            ruleBuilder
                .IsNotTooLong(MaxLengthConstants.UserAgent, "User agent must be at most {MaxLength} characters long.");
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
            ruleBuilder.MinimumLength(minimumLength).WithMessage(message).WithErrorCode("SOME-CODE"); // TODO: think about error codes
    }
}