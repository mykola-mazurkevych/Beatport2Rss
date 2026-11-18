using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Errors;

public sealed class AggregateError(IEnumerable<IError> errors) :
    Error($"Multiple errors occured:\n{string.Join('\n', errors.Select(error => error.Message))}");