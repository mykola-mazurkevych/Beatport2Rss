#pragma warning disable CA1032 // Implement standard exception constructors

namespace Beatport2Rss.Common.Domain.Exceptions;

public sealed class InvalidValueObjectValueException(string message) : Exception(message);