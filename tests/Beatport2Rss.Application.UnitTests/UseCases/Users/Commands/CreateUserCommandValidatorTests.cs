using System.Linq.Expressions;

using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.Contracts.Persistence.Repositories;
using Beatport2Rss.Domain.Users;

using Moq;

using Xunit;

namespace Beatport2Rss.Application.UnitTests.UseCases.Users.Commands;

public sealed class CreateUserCommandValidatorTests
{
    private readonly Mock<IQueryRepository<User, UserId>> _userQueryRepositoryMock;
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _userQueryRepositoryMock = new Mock<IQueryRepository<User, UserId>>();
        _validator = new CreateUserCommandValidator(_userQueryRepositoryMock.Object);
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldPass()
    {
        _userQueryRepositoryMock
            .Setup(r => r.NotExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateUserCommand(
            Username: "validuser",
            EmailAddress: "user@example.com",
            Password: "validpassword123");

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
        _userQueryRepositoryMock
            .Setup(r => r.NotExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

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
        _userQueryRepositoryMock
            .Setup(r => r.NotExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

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
        _userQueryRepositoryMock
            .Setup(r => r.NotExistsAsync(u => u.Username == "existinguser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateUserCommand(
            Username: "existinguser",
            EmailAddress: "user@example.com",
            Password: "validpassword123");

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var usernameError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.Username));
        Assert.Equal("Username is already taken.", usernameError.ErrorMessage);
    }

    [Fact]
    public async Task Validate_UsernameAlreadyTaken_DifferentUsername_ShouldFail()
    {
        _userQueryRepositoryMock
            .Setup(r => r.NotExistsAsync(u => u.Username == "takenuser", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateUserCommand(
            Username: "takenuser",
            EmailAddress: "newuser@example.com",
            Password: "validpassword123");

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
        _userQueryRepositoryMock
            .Setup(r => r.NotExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

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
        _userQueryRepositoryMock
            .Setup(r => r.NotExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

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
        _userQueryRepositoryMock
            .Setup(r => r.NotExistsAsync(u => u.EmailAddress == "existing@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateUserCommand(
            Username: "validuser",
            EmailAddress: "existing@example.com",
            Password: "validpassword123");

        var result = await _validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        var emailError = Assert.Single(result.Errors, e => e.PropertyName == nameof(CreateUserCommand.EmailAddress));
        Assert.Equal("Email address is already taken.", emailError.ErrorMessage);
    }

    [Fact]
    public async Task Validate_EmailAddressAlreadyTaken_DifferentEmail_ShouldFail()
    {
        _userQueryRepositoryMock
            .Setup(r => r.NotExistsAsync(u => u.EmailAddress == "taken@example.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateUserCommand(
            Username: "newuser",
            EmailAddress: "taken@example.com",
            Password: "validpassword123");

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