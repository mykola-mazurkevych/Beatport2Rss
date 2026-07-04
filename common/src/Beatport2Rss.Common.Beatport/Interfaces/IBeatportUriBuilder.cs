namespace Beatport2Rss.Common.Beatport.Interfaces;

public interface IBeatportUriBuilder
{
    Uri Build(int id, string slug);
}