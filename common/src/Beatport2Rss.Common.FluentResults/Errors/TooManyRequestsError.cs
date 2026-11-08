using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Errors;

public sealed class TooManyRequestsError(string message) :
    Error(message);