using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Errors;

public sealed class UnprocessableError(string message) :
    Error(message);