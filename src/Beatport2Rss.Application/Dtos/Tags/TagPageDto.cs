using System.Linq.Expressions;

using Beatport2Rss.Domain.Common.ValueObjects;
using Beatport2Rss.Domain.Tags;
using Beatport2Rss.SharedKernel.Common;

namespace Beatport2Rss.Application.Dtos.Tags;

public sealed record TagPageDto(
    TagId Id,
    TagName Name,
    Slug Slug,
    DateTimeOffset CreatedAt) :
    IPageDto<TagId>
{
    public static Expression<Func<Tag, TagPageDto>> FromTag =>
        tag => new TagPageDto(
            tag.Id,
            tag.Name,
            tag.Slug,
            tag.CreatedAt);
}