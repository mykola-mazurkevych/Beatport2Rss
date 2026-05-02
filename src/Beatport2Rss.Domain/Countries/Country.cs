using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Domain.Countries;

public sealed class Country :
    IAggregateRoot<CountryCode>
{
    private Country()
    {
    }

    public CountryCode Id { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    public CountryName Name { get; private set; }

    public static Country Create(
        CountryCode id,
        DateTimeOffset createdAt,
        CountryName name) =>
        new()
        {
            Id = id,
            CreatedAt = createdAt,
            Name = name,
        };

}