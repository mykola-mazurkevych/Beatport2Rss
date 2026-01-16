using FluentValidation.Results;

namespace Beatport2Rss.Application.Extensions;

internal static class ValidationFailureExtensions
{
    extension(List<ValidationFailure> failures)
    {
        public Dictionary<string, object> ToMetadata() =>
            failures
                .GroupBy(f => f.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    object (g) => g.Select(f => f.ErrorMessage).Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
    }
}