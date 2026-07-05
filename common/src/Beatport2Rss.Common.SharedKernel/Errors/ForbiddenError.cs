using FluentResults;

namespace Beatport2Rss.Common.SharedKernel.Errors;

public sealed class ForbiddenError(string message) :
    Error(message);