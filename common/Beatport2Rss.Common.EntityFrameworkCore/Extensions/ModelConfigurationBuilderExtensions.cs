using System.Reflection;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Beatport2Rss.Common.EntityFrameworkCore.Extensions;

public static class ModelConfigurationBuilderExtensions
{
    extension(ModelConfigurationBuilder builder)
    {
        public void ConfigureConversions() =>
            Assembly.GetCallingAssembly()
                .GetTypes()
                .Union(typeof(ModelConfigurationBuilderExtensions).Assembly.GetTypes())
                .Where(type =>
                    type.BaseType is not null &&
                    type.BaseType.IsGenericType &&
                    type.BaseType.GetGenericTypeDefinition() == typeof(ValueConverter<,>))
                .ToList()
                .ForEach(converterType => builder.Properties(converterType.BaseType!.GetGenericArguments()[0]).HaveConversion(converterType));
    }
}