using FluentResults;

namespace Beatport2Rss.SharedKernel.Errors;

public sealed class UnprocessableError(string message) :
    Error(message);