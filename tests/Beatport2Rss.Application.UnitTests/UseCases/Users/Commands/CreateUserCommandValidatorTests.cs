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
        _emailAddressAvailabilityCheckerMock = new Mock<IEmailAddressAvailabilityChecker>(MockBehavior.Strict);
        _usernameAvailabilityCheckerMock = new Mock<IUsernameAvailabilityChecker>(MockBehavior.Strict);
        _validator = new CreateUserCommandValidator(_emailAddressAvailabilityCheckerMock.Object, _usernameAvailabilityCheckerMock.Object);
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateUserCommand(
            Username: "username",
            EmailAddress: "username@beatport2rss.com",
            Password: "validpassword123");

        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username@beatport2rss.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _usernameAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_UsernameIsNullOrWhitespace_ShouldFail(string? username)
    {
        var command = new CreateUserCommand(
            Username: username,
            EmailAddress: "username@beatport2rss.com",
            Password: "validpassword123");

        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username@beatport2rss.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.Username), failure.PropertyName);
        Assert.Equal("Username is required.", failure.ErrorMessage);
    }

    [Fact]
    public async Task Validate_UsernameTooLong_ShouldFail()
    {
        var command = new CreateUserCommand(
            Username: new string('a', Username.MaxLength + 1),
            EmailAddress: "username@beatport2rss.com",
            Password: "validpassword123");

        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username@beatport2rss.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.Username), failure.PropertyName);
        Assert.Equal($"Username must be at most {Username.MaxLength} characters.", failure.ErrorMessage);
    }

    [Theory]
    [InlineData("user@name")]
    [InlineData("user name")]
    [InlineData("user.name!")]
    public async Task Validate_UsernameContainsInvalidCharacters_ShouldFail(string username)
    {
        var command = new CreateUserCommand(
            Username: username,
            EmailAddress: "username@beatport2rss.com",
            Password: "validpassword123");
        
        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username@beatport2rss.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.Username), failure.PropertyName);
        Assert.Equal("Username contains invalid characters.", failure.ErrorMessage);
    }

    [Fact]
    public async Task Validate_UsernameAlreadyTaken_ShouldFail()
    {
        var command = new CreateUserCommand(
            Username: "username",
            EmailAddress: "username@beatport2rss.com",
            Password: "validpassword123");

        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username@beatport2rss.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        _usernameAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.Username), failure.PropertyName);
        Assert.Equal("Username is already taken.", failure.ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_EmailAddressIsNullOrWhitespace_ShouldFail(string? emailAddress)
    {
        var command = new CreateUserCommand(
            Username: "username",
            EmailAddress: emailAddress,
            Password: "validpassword123");

        _usernameAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.EmailAddress), failure.PropertyName);
        Assert.Equal("Email address is required.", failure.ErrorMessage);
    }

    [Fact]
    public async Task Validate_EmailAddressTooLong_ShouldFail()
    {
        var command = new CreateUserCommand(
            Username: "username",
            EmailAddress: new string('a', EmailAddress.MaxLength + 1) + "@beatport2rss.com",
            Password: "validpassword123");
        
        _usernameAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.EmailAddress), failure.PropertyName);
        Assert.Equal($"Email address must be at most {EmailAddress.MaxLength} characters.", failure.ErrorMessage);
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("user@")]
    [InlineData("@beatport2rss.com")]
    [InlineData("user.beatport2rss.com")]
    public async Task Validate_EmailAddressInvalidFormat_ShouldFail(string emailAddress)
    {
        var command = new CreateUserCommand(
            Username: "username",
            EmailAddress: emailAddress,
            Password: "validpassword123");
        
        _usernameAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.EmailAddress), failure.PropertyName);
        Assert.Equal("A valid email address is required.", failure.ErrorMessage);
    }

    [Fact]
    public async Task Validate_EmailAddressAlreadyTaken_ShouldFail()
    {
        var command = new CreateUserCommand(
            Username: "username",
            EmailAddress: "username@beatport2rss.com",
            Password: "validpassword123");
        
        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username@beatport2rss.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _usernameAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.EmailAddress), failure.PropertyName);
        Assert.Equal("Email address is already taken.", failure.ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_PasswordIsNullOrEmpty_ShouldFail(string? password)
    {
        var command = new CreateUserCommand(
            Username: "username",
            EmailAddress: "username@beatport2rss.com",
            Password: password);
        
        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username@beatport2rss.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _usernameAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.Password), failure.PropertyName);
        Assert.Equal("Password is required.", failure.ErrorMessage);
    }

    [Theory]
    [InlineData("1234567")]
    [InlineData("abc")]
    public async Task Validate_PasswordTooShort_ShouldFail(string password)
    {
        var command = new CreateUserCommand(
            Username: "username",
            EmailAddress: "username@beatport2rss.com",
            Password: password);
        
        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username@beatport2rss.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _usernameAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.Password), failure.PropertyName);
        Assert.Equal($"Password must be at least {Password.MinLength} characters long.", failure.ErrorMessage);
    }

    [Fact]
    public async Task Validate_PasswordTooLong_ShouldFail()
    {
        var command = new CreateUserCommand(
            Username: "username",
            EmailAddress: "username@beatport2rss.com",
            Password: new string('a', Password.MaxLength + 1));

        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username@beatport2rss.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _usernameAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("username", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.Password), failure.PropertyName);
        Assert.Equal($"Password must be at most {Password.MaxLength} characters.", failure.ErrorMessage);
    }
}