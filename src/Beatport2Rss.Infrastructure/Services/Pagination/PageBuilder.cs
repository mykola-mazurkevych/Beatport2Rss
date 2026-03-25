using System.Linq.Expressions;

using Beatport2Rss.Application.Interfaces.Pagination;
using Beatport2Rss.Application.Pagination;
using Beatport2Rss.SharedKernel.Common;

using EFPagination;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Services.Pagination;

internal sealed class PageBuilder(
    ICursorEncoder cursorEncoder) :
    IPageBuilder
{
    private const int DefaultSize = 10;

    public async Task<Page<TPageDto>> BuildAsync<TEntity, TId, TPageDto>(
        IQueryable<TEntity> entities,
        PageNavigation navigation,
        Expression<Func<TEntity, TPageDto>> selector,
        CancellationToken cancellationToken = default)
        where TEntity : class, IEntity<TId>
        where TId : struct, IId<TId>
        where TPageDto : IPageDto<TId>
    {
        var pageSize = navigation.PageSize ?? DefaultSize;
        var totalCount = await entities.CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return new Page<TPageDto>([], PageMetadata.Empty(pageSize));
        }

        var definition = PaginationQuery.Build<TEntity>(builder => builder.Ascending(dto => dto.CreatedAt).Ascending(dto => dto.Id));

        var nextCursor = cursorEncoder.Decode<TId>(navigation.NextPage);
        var previousCursor = cursorEncoder.Decode<TId>(navigation.PreviousPage);

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

        var metadata = new PageMetadata(
            pageSize,
            pageEntities.Count,
            totalCount,
            await context.HasNextAsync(pageEntities)
                ? cursorEncoder.Encode(new Cursor<TId>(pageEntities[^1].CreatedAt, pageEntities[^1].Id))
                : null,
            await context.HasPreviousAsync(pageEntities)
                ? cursorEncoder.Encode(new Cursor<TId>(pageEntities[0].CreatedAt, pageEntities[0].Id))
                : null);

        return new Page<TPageDto>(pageEntities.AsReadOnly(), metadata);
    }
}