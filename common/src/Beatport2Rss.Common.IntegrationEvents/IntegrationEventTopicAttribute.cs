namespace Beatport2Rss.Common.IntegrationEvents;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class IntegrationEventTopicAttribute(string topicName) : Attribute
{
    public string TopicName { get; } = string.IsNullOrWhiteSpace(topicName)
        ? throw new ArgumentException("Topic name cannot be empty.", nameof(topicName))
        : topicName;
}