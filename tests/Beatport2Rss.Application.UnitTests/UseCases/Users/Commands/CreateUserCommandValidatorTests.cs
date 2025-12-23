
using Beatport2Rss.Application.Interfaces.Services;
using Beatport2Rss.Application.UseCases.Users.Commands;
using Beatport2Rss.Domain.Users;

using Moq;

using Xunit;

namespace Beatport2Rss.Application.UnitTests.UseCases.Users.Commands;

public sealed class CreateUserCommandValidatorTests
{
    private readonly Mock<IEmailAddressAvailabilityChecker> _emailAddressAvailabilityCheckerMock;
    private readonly CreateUserCommandValidator _validator;

    public CreateUserCommandValidatorTests()
    {
        _emailAddressAvailabilityCheckerMock = new Mock<IEmailAddressAvailabilityChecker>(MockBehavior.Strict);
        _validator = new CreateUserCommandValidator(_emailAddressAvailabilityCheckerMock.Object);
    }

    [Fact]
    public async Task Validate_ValidCommand_ShouldPass()
    {
        var command = new CreateUserCommand("email@address.com", "validpassword123");

        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("email@address.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validate_EmailAddressIsNullOrWhitespace_ShouldFail(string? emailAddress)
    {
        var command = new CreateUserCommand(emailAddress, "validpassword123");

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.EmailAddress), failure.PropertyName);
        Assert.Equal("Email address is required.", failure.ErrorMessage);
    }

    [Fact]
    public async Task Validate_EmailAddressTooLong_ShouldFail()
    {
        var command = new CreateUserCommand(new string('e', EmailAddress.MaxLength + 1) + "@address.com", "validpassword123");
        
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.EmailAddress), failure.PropertyName);
        Assert.Equal($"Email address must be at most {EmailAddress.MaxLength} characters.", failure.ErrorMessage);
    }

    [Theory]
    [InlineData("invalidemail")]
    [InlineData("email@")]
    [InlineData("@address.com")]
    [InlineData("email.address.com")]
    public async Task Validate_EmailAddressInvalidFormat_ShouldFail(string emailAddress)
    {
        var command = new CreateUserCommand(emailAddress, "validpassword123");
        
        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.EmailAddress), failure.PropertyName);
        Assert.Equal("A valid email address is required.", failure.ErrorMessage);
    }

    [Fact]
    public async Task Validate_EmailAddressAlreadyTaken_ShouldFail()
    {
        var command = new CreateUserCommand("email@address.com", "validpassword123");
        
        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("email@address.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

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
        var command = new CreateUserCommand("email@address.com", password);
        
        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("email@address.com", It.IsAny<CancellationToken>()))
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
        var command = new CreateUserCommand("email@address.com", password);
        
        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("email@address.com", It.IsAny<CancellationToken>()))
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
        var command = new CreateUserCommand("email@address.com", new string('a', Password.MaxLength + 1));

        _emailAddressAvailabilityCheckerMock
            .Setup(c => c.IsAvailableAsync("email@address.com", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _validator.ValidateAsync(command, TestContext.Current.CancellationToken);

        Assert.False(result.IsValid);
        var failure = Assert.Single(result.Errors);
        Assert.Equal(nameof(CreateUserCommand.Password), failure.PropertyName);
        Assert.Equal($"Password must be at most {Password.MaxLength} characters.", failure.ErrorMessage);
    }
}