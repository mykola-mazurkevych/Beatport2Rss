using Beatport2Rss.Domain.Feeds;

namespace Beatport2Rss.Domain.Common.Exceptions;

public sealed class FeedNameIsTakenException(FeedName name) :
    BusinessRuleViolationException($"Feed name '{name}' is already taken.");