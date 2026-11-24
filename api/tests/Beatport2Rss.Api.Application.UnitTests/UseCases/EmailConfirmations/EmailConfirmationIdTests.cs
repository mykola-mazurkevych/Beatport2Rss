using Beatport2Rss.Api.Domain.EmailConfirmations;

using Xunit;

namespace Beatport2Rss.Api.Application.UnitTests.UseCases.EmailConfirmations;

public sealed class EmailConfirmationIdTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("not-a-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public void TryParse_WhenInvalid_ShouldReturnFalse(string? value)
    {
        Assert.False(EmailConfirmationId.TryParse(value, null, out _));
    }

    [Fact]
    public void TryParse_WhenValid_ShouldReturnId()
    {
        var value = Guid.NewGuid();

        Assert.True(EmailConfirmationId.TryParse(value.ToString(), null, out var result));
        Assert.Equal(value, result.Value);
    }
}
