using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Errors;

public sealed class UnauthorizedError(string message) :
    Error(message);