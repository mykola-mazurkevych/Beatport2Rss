using FluentResults;

namespace Beatport2Rss.SharedKernel.Errors;

public sealed class ConflictError(string message) 
    : Error(message);