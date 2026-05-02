using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Infrastructure.Persistence.Extensions;

internal static class PropertyBuilderExtensions
{
    private const int EnumMaxLength = 50;

    extension<TEnum>(PropertyBuilder<TEnum> builder)
        where TEnum : struct, Enum
    {
        public PropertyBuilder<TEnum> IsEnum() =>
            builder
                .HasConversion<EnumToStringConverter<TEnum>>()
                .HasMaxLength(EnumMaxLength)
                .IsRequired();
    }

    extension(PropertyBuilder<Uri> builder)
    {
        public PropertyBuilder<Uri> IsUri() =>
            builder
                .HasConversion<string>()
                .HasMaxLength(500)
                .IsRequired();
    }
}