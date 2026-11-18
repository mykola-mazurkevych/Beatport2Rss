using Beatport2Rss.Common.FluentResults.Errors;
using Beatport2Rss.Common.FluentResults.Successes;

using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Extensions;

public static class ResultBaseExtensions
{
    extension(IResultBase result)
    {
        public IError Error =>
            result.Errors.Count switch
            {
                0 => throw new InvalidOperationException(),
                1 => result.Errors.Single(),
                _ => new AggregateError(result.Errors)
            };

        public bool HasRetryableError =>
            result.Errors.Any(error => error is TransientError or TransientHttpError or RequestTimeoutError or TooManyRequestsError);

        public bool IsNotModified =>
            result.IsSuccess && result.Successes.Any(success => success is NotModifiedSuccess);

        public bool TryGetSuccessMetadata(string key, out object? value)
        {
            value = null;
            return
                result.IsSuccess &&
                result.Successes.Count == 1 &&
                result.Successes.Single().Metadata.TryGetValue(key, out value);
        }
    }
}