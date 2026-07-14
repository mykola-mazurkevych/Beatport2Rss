using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Application.Interfaces.Services.Misc;

public interface ISlugGenerator
{
    Slug Generate(string name);
}