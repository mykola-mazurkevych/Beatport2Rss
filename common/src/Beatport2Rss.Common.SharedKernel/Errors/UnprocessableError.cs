using FluentResults;

namespace Beatport2Rss.Common.SharedKernel.Errors;

public sealed class UnprocessableError(string message) :
    Error(message);