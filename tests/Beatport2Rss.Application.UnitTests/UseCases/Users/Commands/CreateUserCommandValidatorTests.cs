using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.Domain.Users;

using Xunit;

namespace Beatport2Rss.Application.UnitTests.UseCases.Users.Commands;

public sealed class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    [Fact]
    public async Task Validate_WithNoNames_ShouldSucceed()
    {
        var command = new CreateUserCommand("email@address.com", "validpassword123", null, null);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validate_WithNames_ShouldSucceed()
    {
        var command = new CreateUserCommand("email@address.com", "validpassword123", "First", "Last");

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_WhenEmailAddressIsNullOrWhitespace_ShouldFail(string? emailAddress)
    {
        var command = new CreateUserCommand(emailAddress, "validpassword123", null, null);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.EmailAddress), failure.PropertyName);
        Assert.Equal("Email address is required.", failure.ErrorMessage);
    }

    [Fact]
    public async Task Validate_WhenEmailAddressTooLong_ShouldFail()
    {
        var command = new CreateUserCommand(new string('e', EmailAddress.MaxLength + 1) + "@address.com", "validpassword123", null, null);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.EmailAddress), failure.PropertyName);
        Assert.Equal($"Email address must be at most {EmailAddress.MaxLength} characters long.", failure.ErrorMessage);
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("email@")]
    [InlineData("@address.com")]
    [InlineData("email.address.com")]
    public async Task Validate_WhenEmailAddressInvalidFormat_ShouldFail(string emailAddress)
    {
        var command = new CreateUserCommand(emailAddress, "validpassword123", null, null);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.EmailAddress), failure.PropertyName);
        Assert.Equal("A valid email address is required.", failure.ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_WhenPasswordIsNullOrEmpty_ShouldFail(string? password)
    {
        var command = new CreateUserCommand("email@address.com", password, null, null);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.Password), failure.PropertyName);
        Assert.Equal("Password is required.", failure.ErrorMessage);
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("abc")]
    public async Task Validate_WhenPasswordTooShort_ShouldFail(string password)
    {
        var command = new CreateUserCommand("email@address.com", password, null, null);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.Password), failure.PropertyName);
        Assert.Equal($"Password must be at least {Password.MinLength} characters long.", failure.ErrorMessage);
    }

    [Fact]
    public async Task Validate_WhenPasswordTooLong_ShouldFail()
    {
        var command = new CreateUserCommand("email@address.com", new string('a', Password.MaxLength + 1), null, null);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.Password), failure.PropertyName);
        Assert.Equal($"Password must be at most {Password.MaxLength} characters long.", failure.ErrorMessage);
    }

    [Fact]
    public async Task Validate_WhenFirstNameTooLong_ShouldFail()
    {
        var command = new CreateUserCommand("email@address.com", "validpassword123", new string('f', User.NameLength + 1), null);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.FirstName), failure.PropertyName);
        Assert.Equal($"First name must be at most {User.NameLength} characters long.", failure.ErrorMessage);
    }

    [Fact]
    public async Task Validate_WhenLastNameTooLong_ShouldFail()
    {
        var command = new CreateUserCommand("email@address.com", "validpassword123", null, new string('l', User.NameLength + 1));

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.LastName), failure.PropertyName);
        Assert.Equal($"Last name must be at most {User.NameLength} characters long.", failure.ErrorMessage);
    }
}