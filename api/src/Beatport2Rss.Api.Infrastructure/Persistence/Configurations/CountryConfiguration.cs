using Beatport2Rss.Api.Domain.Countries;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Beatport2Rss.Api.Infrastructure.Persistence.Configurations;

internal sealed class CountryConfiguration :
    IEntityTypeConfiguration<Country>
{
    public void Configure(EntityTypeBuilder<Country> builder)
    {
        builder.ToTable(nameof(ApiDbContext.Countries));

        builder.HasKey(country => country.Id);

        builder.Property(country => country.Id)
            .HasMaxLength(CountryCode.Length)
            .IsRequired();

        builder.Property(country => country.CreatedAt)
            .IsRequired();

        builder.Property(country => country.Name)
            .HasMaxLength(CountryName.MaxLength)
            .IsRequired();
    }
}