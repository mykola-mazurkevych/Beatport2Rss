#pragma warning disable CA1032 // Implement standard exception constructors

namespace Beatport2Rss.Common.SharedKernel.Exceptions;

public sealed class InvalidValueObjectValueException(string message) : Exception(message);