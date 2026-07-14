using Beatport2Rss.Api.Domain.Tags;
using Beatport2Rss.Api.Domain.Users;
using Beatport2Rss.Common.SharedKernel.Interfaces;
using Beatport2Rss.Common.SharedKernel.ValueObjects;

namespace Beatport2Rss.Api.Infrastructure.Persistence.QueryModels;

internal sealed record TagQueryModel(
    TagId Id,
    DateTimeOffset CreatedAt,
    UserId UserId,
    TagName Name,
    Slug Slug) :
    IQueryModel<TagId>;