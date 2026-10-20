using Beatport2Rss.Common.Domain.ValueObjects;

namespace Beatport2Rss.Api.Application.Interfaces.Services.Misc;

public interface ISlugGenerator
{
    Slug Generate(string name);
}