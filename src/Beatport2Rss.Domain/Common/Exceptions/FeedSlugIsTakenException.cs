using Beatport2Rss.Domain.Common.ValueObjects;

namespace Beatport2Rss.Domain.Common.Exceptions;

public sealed class FeedSlugIsTakenException(Slug slug) :
    BusinessRuleViolationException($"Feed slug '{slug}' is already taken.");