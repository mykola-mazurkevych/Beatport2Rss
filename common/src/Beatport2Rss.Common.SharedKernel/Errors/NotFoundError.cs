using FluentResults;

namespace Beatport2Rss.Common.SharedKernel.Errors;

public sealed class NotFoundError(string message) :
    Error(message);