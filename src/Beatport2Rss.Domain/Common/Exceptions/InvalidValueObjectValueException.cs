namespace Beatport2Rss.Domain.Common.Exceptions;

public sealed class InvalidValueObjectValueException(string message) : Exception(message);