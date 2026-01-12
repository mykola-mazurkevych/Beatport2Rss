using ErrorOr;

using FluentValidation.Results;

namespace Beatport2Rss.WebApi.Extensions;

internal static class ErrorExtensions
{
    extension(Error error)
    {
        public IDictionary<string, string[]> GetErrors()
        {
            IDictionary<string, string[]> errors = new Dictionary<string, string[]>();

            if (error.Metadata is not null &&
                error.Metadata.TryGetValue(nameof(ValidationResult), out var validationResultObj) &&
                validationResultObj is ValidationResult validationResult)
            {
                errors = validationResult.Errors
                    .GroupBy(f => f.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(f => f.ErrorMessage).ToArray())
                    .AsReadOnly();
            }

            return errors;
        }
    }
}