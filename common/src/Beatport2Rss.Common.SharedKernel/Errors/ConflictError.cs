using FluentResults;

namespace Beatport2Rss.Common.SharedKernel.Errors;

public sealed class ConflictError(string message) :
    Error(message);