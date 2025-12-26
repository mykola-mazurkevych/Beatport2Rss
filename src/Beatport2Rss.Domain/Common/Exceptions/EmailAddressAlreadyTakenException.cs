#pragma warning disable CA1032 // Implement standard exception constructors

namespace Beatport2Rss.Domain.Common.Exceptions;

public sealed class EmailAddressAlreadyTakenException(string title, string? detail = null) : ConflictException(title, detail);