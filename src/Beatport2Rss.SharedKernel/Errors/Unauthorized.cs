using FluentResults;

namespace Beatport2Rss.SharedKernel.Errors;

public sealed class Unauthorized(string message) 
    : Error(message);