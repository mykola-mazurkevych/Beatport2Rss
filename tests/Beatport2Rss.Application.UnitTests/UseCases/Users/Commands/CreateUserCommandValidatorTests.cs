using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.Domain.Users;

using Moq;

using Xunit;

namespace Beatport2Rss.Application.UnitTests.UseCases.Users.Commands;

public sealed class CreateUserCommandValidatorTests
{
    private readonly Mock<IEmailAddressAvailabilityChecker> _emailAddressAvailabilityCheckerMock;
    private readonly Mock<IUsernameAvailabilityChecker> _usernameAvailabilityCheckerMock;
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _emailAddressAvailabilityCheckerMock = new Mock<IEmailAddressAvailabilityChecker>();
        _usernameAvailabilityCheckerMock = new Mock<IUsernameAvailabilityChecker>();
        _validator = new CreateUserCommandValidator(_emailAddressAvailabilityCheckerMock.Object, _usernameAvailabilityCheckerMock.Object);
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateUserCommand(
            Username: "validuser",
            EmailAddress: "user@example.com",
            Password: "validpassword123");

        _emailAddressAvailabilityCheckerMock
            .Setup(r => r.IsAvailableAsync("user@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _usernameAvailabilityCheckerMock
            .Setup(r => r.IsAvailableAsync("validuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_UsernameIsNullOrEmpty_ShouldFail(string? username)
    {
        var command = new CreateUserCommand(
            Username: username,
            EmailAddress: "user@example.com",
            Password: "validpassword123");

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var usernameError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.Username));
        Assert.Equal("Username is required.", usernameError.ErrorMessage);
    }

    [Fact]
    public async Task Validate_UsernameTooLong_ShouldFail()
    {
        var longUsername = new string('a', Username.MaxLength + 1);
        var command = new CreateUserCommand(
            Username: longUsername,
            EmailAddress: "user@example.com",
            Password: "validpassword123");

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var usernameError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.Username));
        Assert.Equal($"Username must be at most {Username.MaxLength} characters.", usernameError.ErrorMessage);
    }

    [Theory]
    [InlineData("user@name")]
    [InlineData("user name")]
    [InlineData("user.name!")]
    public async Task Validate_UsernameContainsInvalidCharacters_ShouldFail(string username)
    {
        var command = new CreateUserCommand(
            Username: username,
            EmailAddress: "user@example.com",
            Password: "validpassword123");

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var usernameError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.Username));
        Assert.Equal("Username contains invalid characters.", usernameError.ErrorMessage);
    }

    [Fact]
    public async Task Validate_UsernameAlreadyTaken_ShouldFail()
    {
        var command = new CreateUserCommand(
            Username: "existinguser",
            EmailAddress: "user@example.com",
            Password: "validpassword123");

        _usernameAvailabilityCheckerMock
            .Setup(r => r.IsAvailableAsync("existinguser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var usernameError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.Username));
        Assert.Equal("Username is already taken.", usernameError.ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_EmailAddressIsNullOrEmpty_ShouldFail(string? emailAddress)
    {
        var command = new CreateUserCommand(
            Username: "validuser",
            EmailAddress: emailAddress,
            Password: "validpassword123");

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var emailError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.EmailAddress));
        Assert.Equal("Email address is required.", emailError.ErrorMessage);
    }

    [Fact]
    public async Task Validate_EmailAddressTooLong_ShouldFail()
    {
        var longEmail = new string('a', EmailAddress.MaxLength - 10) + "@example.com";
        var command = new CreateUserCommand(
            Username: "validuser",
            EmailAddress: longEmail,
            Password: "validpassword123");

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var emailError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.EmailAddress));
        Assert.Equal($"Email address must be at most {EmailAddress.MaxLength} characters.", emailError.ErrorMessage);
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("user@")]
    [InlineData("@example.com")]
    [InlineData("user.example.com")]
    public async Task Validate_EmailAddressInvalidFormat_ShouldFail(string emailAddress)
    {
        var command = new CreateUserCommand(
            Username: "validuser",
            EmailAddress: emailAddress,
            Password: "validpassword123");

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var emailError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.EmailAddress));
        Assert.Equal("A valid email address is required.", emailError.ErrorMessage);
    }

    [Fact]
    public async Task Validate_EmailAddressAlreadyTaken_ShouldFail()
    {
        var command = new CreateUserCommand(
            Username: "validuser",
            EmailAddress: "existing@example.com",
            Password: "validpassword123");
        
        _emailAddressAvailabilityCheckerMock
            .Setup(r => r.IsAvailableAsync("existing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var emailError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.EmailAddress));
        Assert.Equal("Email address is already taken.", emailError.ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_PasswordIsNullOrEmpty_ShouldFail(string? password)
    {
        var command = new CreateUserCommand(
            Username: "validuser",
            EmailAddress: "user@example.com",
            Password: password);

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var passwordError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.Password));
        Assert.Equal("Password is required.", passwordError.ErrorMessage);
    }

    [Theory]
    [InlineData("1234567")] // 7 characters
    [InlineData("abc")]     // 3 characters
    public async Task Validate_PasswordTooShort_ShouldFail(string password)
    {
        var command = new CreateUserCommand(
            Username: "validuser",
            EmailAddress: "user@example.com",
            Password: password);

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var passwordError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.Password));
        Assert.Equal("Password must be at least 8 characters long.", passwordError.ErrorMessage);
    }
}