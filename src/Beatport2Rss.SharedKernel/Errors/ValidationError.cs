using FluentResults;

namespace Beatport2Rss.SharedKernel.Errors;

public sealed class ValidationError(string message) 
    : Error(message);