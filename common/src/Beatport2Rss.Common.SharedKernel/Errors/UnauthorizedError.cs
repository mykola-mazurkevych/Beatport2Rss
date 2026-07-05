using FluentResults;

namespace Beatport2Rss.Common.SharedKernel.Errors;

public sealed class UnauthorizedError(string message) :
    Error(message);