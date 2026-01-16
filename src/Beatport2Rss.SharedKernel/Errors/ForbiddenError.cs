using FluentResults;

namespace Beatport2Rss.SharedKernel.Errors;

public sealed class ForbiddenError(string message) 
    : Error(message);