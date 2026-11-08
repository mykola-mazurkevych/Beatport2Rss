using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Errors;

public sealed class TransientError(string message) :
    Error(message);