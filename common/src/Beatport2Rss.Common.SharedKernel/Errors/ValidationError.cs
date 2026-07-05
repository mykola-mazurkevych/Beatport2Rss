using FluentResults;

namespace Beatport2Rss.Common.SharedKernel.Errors;

public sealed class ValidationError(string message) :
    Error(message);