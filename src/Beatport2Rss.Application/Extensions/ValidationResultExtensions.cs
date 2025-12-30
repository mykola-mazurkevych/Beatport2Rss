using FluentValidation.Results;

namespace Beatport2Rss.Application.Extensions;

internal static class ValidationResultExtensions
{
    extension(ValidationResult result)
    {
        public IDictionary<string, string[]> GetErrors() => result.Errors
            .GroupBy(f => f.PropertyName)
            .ToDictionary(
                g => g.Key,
                g => g.Select(f => f.ErrorMessage).ToArray())
            .AsReadOnly();
    }
}