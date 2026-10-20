using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Errors;

public sealed class NotFoundError(string message) :
    Error(message);