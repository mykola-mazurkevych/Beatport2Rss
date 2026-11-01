using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Errors;

public sealed class ForbiddenError(string message) :
    Error(message);