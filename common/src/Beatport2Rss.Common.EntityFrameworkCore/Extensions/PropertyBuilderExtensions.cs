using Beatport2Rss.Common.SharedKernel.Constants;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.Extensions;

public static class PropertyBuilderExtensions
{
    extension<TEnum>(PropertyBuilder<TEnum> builder)
        where TEnum : struct, Enum
    {
        public PropertyBuilder<TEnum> IsEnum() =>
            builder
                .HasConversion<EnumToStringConverter<TEnum>>()
                .HasMaxLength(MaxLengthConstants.Enum)
                .IsRequired();
    }

    extension(PropertyBuilder<Uri> builder)
    {
        public PropertyBuilder<Uri> IsUri() =>
            builder
                .HasConversion<string>()
                .HasMaxLength(MaxLengthConstants.Uri)
                .IsRequired();
    }
}