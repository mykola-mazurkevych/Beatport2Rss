using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beatport2Rss.Infrastructure.Extensions;

internal static class JsonSerializerOptionsExtensions
{
    extension(JsonSerializerOptions options)
    {
        public void ConfigureDefault()
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.WriteIndented = true;
            options.AddJsonValueConverters();
        }

        public static string Plain => "Plain";

        public void ConfigurePlain()
        {
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.AddJsonValueConverters();
        }

        private void AddJsonValueConverters()
        {
            options.Converters.Add(new JsonStringEnumConverter());
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
}