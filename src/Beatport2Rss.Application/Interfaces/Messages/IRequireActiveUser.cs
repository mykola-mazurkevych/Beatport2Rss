namespace Beatport2Rss.Application.Interfaces.Messages;

public interface IRequireActiveUser
{
    Guid UserId { get; }
}