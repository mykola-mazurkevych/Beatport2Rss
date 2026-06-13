using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.Extensions;

public static class PropertyBuilderExtensions
{
    private const int EnumMaxLength = 50;
    private const int UriMaxLength = 500;

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
                .HasMaxLength(UriMaxLength)
                .IsRequired();
    }
}