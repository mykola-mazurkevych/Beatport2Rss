using Beatport2Rss.Application.Interfaces.Pagination;
using Beatport2Rss.Application.Pagination;
using Beatport2Rss.SharedKernel.Common;

using Microsoft.EntityFrameworkCore;

namespace Beatport2Rss.Infrastructure.Services.Pagination;

internal sealed class PageBuilder(
    ICursorEndcoder cursorEndcoder) :
    IPageBuilder
{
    private const int DefaultSize = 10;

    public async Task<Page<TDto>> BuildAsync<TEntity, TId, TDto>(
        IQueryable<TEntity> entitiesAsQueryable,
        int? size,
        string? next,
        string? previous,
        Func<TEntity, TDto> dtoSelector,
        CancellationToken cancellationToken = default)
        where TEntity : IEntity<TId>
        where TId : struct, IId<TId>
    {
        var page = new Page<TDto>
        {
            Size = size ?? DefaultSize,
            TotalCount = await entitiesAsQueryable.CountAsync(cancellationToken)
        };

        if (page.TotalCount == 0)
        {
            return page;
        }

        var nextCursor = cursorEndcoder.Decode<TId>(next);
        var previousCursor = cursorEndcoder.Decode<TId>(previous);

        if (nextCursor is not null)
        {
            entitiesAsQueryable = entitiesAsQueryable is IOrderedQueryable<TEntity> orderedEntitiesAsQueryable
                ? orderedEntitiesAsQueryable.ThenBy(e => e.CreatedAt).ThenBy(e => e.Id)
                : entitiesAsQueryable.OrderBy(e => e.CreatedAt).ThenBy(e => e.Id);

            entitiesAsQueryable = entitiesAsQueryable
                .Where(f =>
                    f.CreatedAt > nextCursor.CreatedAt ||
                    (f.CreatedAt == nextCursor.CreatedAt && f.Id.CompareTo(nextCursor.Id) == 1));
        }
        else if (previousCursor is not null)
        {
            entitiesAsQueryable = entitiesAsQueryable is IOrderedQueryable<TEntity> orderedEntitiesAsQueryable
                ? orderedEntitiesAsQueryable.ThenByDescending(e => e.CreatedAt).ThenByDescending(e => e.Id)
                : entitiesAsQueryable.OrderByDescending(e => e.CreatedAt).ThenByDescending(e => e.Id);

            entitiesAsQueryable = entitiesAsQueryable
                .Where(f =>
                    f.CreatedAt < previousCursor.CreatedAt ||
                    (f.CreatedAt == previousCursor.CreatedAt && f.Id.CompareTo(previousCursor.Id) == -1));
        }

        var entities = await entitiesAsQueryable
            .Take(page.Size + 1)
            .ToListAsync(cancellationToken);

        var hasMore = entities.Count > page.Size;

        if (hasMore)
        {
            entities.RemoveAt(entities.Count - 1);
        }

        if (previousCursor is not null)
        {
            entities.Reverse();
        }

        if (entities.Count == 0)
        {
            return page;
        }

        page = page with { Items = entities.Select(dtoSelector).ToList().AsReadOnly() };

        var firstEntity = entities.First();
        var lastEntity = entities.Last();

        if (nextCursor is not null)
        {
            page = page with { Previous = cursorEndcoder.Encode(new Cursor<TId>(firstEntity.CreatedAt, firstEntity.Id)) };

            if (hasMore)
            {
                page = page with { Next = cursorEndcoder.Encode(new Cursor<TId>(lastEntity.CreatedAt, lastEntity.Id)) };
            }
        }
        else if (previousCursor is not null)
        {
            page = page with { Next = cursorEndcoder.Encode(new Cursor<TId>(lastEntity.CreatedAt, lastEntity.Id)) };

            if (hasMore)
            {
                page = page with { Previous = cursorEndcoder.Encode(new Cursor<TId>(firstEntity.CreatedAt, firstEntity.Id)) };
            }
        }
        else if (hasMore)
        {
            page = page with { Next = cursorEndcoder.Encode(new Cursor<TId>(lastEntity.CreatedAt, lastEntity.Id)) };
        }

        return page;
    }
}