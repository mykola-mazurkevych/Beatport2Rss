#pragma warning disable CA1034 // Nested types should not be visible

using FluentValidation.Results;

namespace Beatport2Rss.Common.Validation.Extensions;

public static class ValidationFailureExtensions
{
    extension(List<ValidationFailure> failures)
    {
        public Dictionary<string, object> ToMetadata() =>
            failures
                .GroupBy(validationFailure => validationFailure.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    object (g) => g.Select(f => f.ErrorMessage).Distinct(StringComparer.OrdinalIgnoreCase).ToArray());
    }
}