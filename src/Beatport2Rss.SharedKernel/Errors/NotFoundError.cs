using FluentResults;

namespace Beatport2Rss.SharedKernel.Errors;

public sealed class NotFoundError(string message) 
    : Error(message);