using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Querying.Paging;
using Beatport2Rss.Application.Querying.Paging;
using Beatport2Rss.SharedKernel.Common;

using EFPagination;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Services.Querying.Paging;

internal sealed class PageBuilder(
    ICursorEncoder cursorEncoder) :
    IPageBuilder
{
    private const int DefaultSize = 10;

    public async Task<Page<TPageDto>> BuildAsync<TEntity, TId, TPageDto>(
        IQueryable<TEntity> entities,
        Pagination pagination,
        Expression<Func<TEntity, TPageDto>> selector,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TId>
        where TId : struct, IId<TId>
        where TPageDto : IPageDto<TId>
    {
        var pageSize = pagination.Size ?? DefaultSize;
        var totalCount = await entities.CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return new Page<TPageDto>([], PageInfo.Empty(pageSize));
        }

        var definition = PaginationQuery.Build<TEntity>(builder => builder.Ascending(dto => dto.CreatedAt).Ascending(dto => dto.Id));

        var nextCursor = cursorEncoder.Decode<TId>(pagination.Next);
        var previousCursor = cursorEncoder.Decode<TId>(pagination.Previous);

        PaginationDirection direction;
        TEntity? reference = null;

        if (previousCursor is null)
        {
            direction = PaginationDirection.Forward;
            if (nextCursor is not null)
            {
                reference = await entities.FirstOrDefaultAsync(dto => dto.Id.Equals(nextCursor.Id), cancellationToken);
            }
        }
        else
        {
            direction = PaginationDirection.Backward;
            reference = await entities.FirstOrDefaultAsync(dto => dto.Id.Equals(previousCursor.Id), cancellationToken);
        }

        var context = entities.Paginate(definition, direction, reference);

        var pageEntities = await context.Query
            .Take(pageSize)
            .Select(selector)
            .ToListAsync(cancellationToken);

        context.EnsureCorrectOrder(pageEntities);

        var pageInfo = new PageInfo(
            pageSize,
            pageEntities.Count,
            totalCount,
            await context.HasNextAsync(pageEntities)
                ? cursorEncoder.Encode(new Cursor<TId>(pageEntities[^1].CreatedAt, pageEntities[^1].Id))
                : null,
            await context.HasPreviousAsync(pageEntities)
                ? cursorEncoder.Encode(new Cursor<TId>(pageEntities[0].CreatedAt, pageEntities[0].Id))
                : null);

        return new Page<TPageDto>(pageEntities.AsReadOnly(), pageInfo);
    }
}