namespace Beatport2Rss.Domain.Common.Exceptions;

public sealed class EmailAddressAlreadyTakenException(string title, string? detail = null) : ConflictException(title, detail);