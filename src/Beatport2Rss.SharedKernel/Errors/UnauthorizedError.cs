using FluentResults;

namespace Beatport2Rss.SharedKernel.Errors;

public sealed class UnauthorizedError(string message) 
    : Error(message);