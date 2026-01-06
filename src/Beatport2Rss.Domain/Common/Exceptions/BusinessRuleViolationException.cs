namespace Beatport2Rss.Domain.Common.Exceptions;

public abstract class BusinessRuleViolationException(string message) : Exception(message);