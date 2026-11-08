#pragma warning disable CA1032 // Implement standard exception constructors

namespace Beatport2Rss.Collector.Application.Exceptions;

public sealed class RetryableBeatportRequestException(string message) :
    Exception(message);