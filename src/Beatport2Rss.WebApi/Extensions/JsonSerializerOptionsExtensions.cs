using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beatport2Rss.WebApi.Extensions;

internal static class JsonSerializerOptionsExtensions
{
    extension(JsonSerializerOptions options)
    {
        public void AddJsonValueConverters() =>
            Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type =>
                    type.BaseType is not null &&
                    type.BaseType.IsGenericType &&
                    type.BaseType.GetGenericTypeDefinition() == typeof(JsonConverter<>))
                .ToList()
                .ForEach(converterType => options.Converters.Add((JsonConverter)Activator.CreateInstance(converterType)!));
    }
}