using Beatport2Rss.Api.Domain.Common.ValueObjects;

namespace Beatport2Rss.Api.Application.Interfaces.Services.Misc;

public interface ISlugGenerator
{
    Slug Generate(string name);
}