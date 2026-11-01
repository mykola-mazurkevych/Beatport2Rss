using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Errors;

public sealed class ValidationError(string message) :
    Error(message);