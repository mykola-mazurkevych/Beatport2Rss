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

    public async Task<Page<TPaginable>> BuildAsync<TPaginable, TId>(
        IQueryable<TPaginable> paginables,
        Pagination pagination,
        CancellationToken cancellationToken = default)
        where TPaginable : class, IPaginable<TId>
        where TId : struct, IId<TId>
    {
        var pageSize = pagination.Size ?? DefaultSize;
        var totalCount = await paginables.CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return new Page<TPaginable>([], PageInfo.Empty(pageSize));
        }

        var definition = PaginationQuery.Build<TPaginable>(builder => builder.Ascending(dto => dto.CreatedAt).Ascending(dto => dto.Id));

        var nextCursor = cursorEncoder.Decode<TId>(pagination.Next);
        var previousCursor = cursorEncoder.Decode<TId>(pagination.Previous);

        PaginationDirection direction;
        TPaginable? reference = null;

        if (previousCursor is null)
        {
            direction = PaginationDirection.Forward;
            if (nextCursor is not null)
            {
                reference = await paginables.FirstOrDefaultAsync(dto => dto.Id.Equals(nextCursor.Id), cancellationToken);
            }
        }
        else
        {
            direction = PaginationDirection.Backward;
            reference = await paginables.FirstOrDefaultAsync(dto => dto.Id.Equals(previousCursor.Id), cancellationToken);
        }

        var context = paginables.Paginate(definition, direction, reference);

        var pagePaginables = await context.Query
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        context.EnsureCorrectOrder(pagePaginables);

        var pageInfo = new PageInfo(
            pageSize,
            pagePaginables.Count,
            totalCount,
            await context.HasNextAsync(pagePaginables)
                ? cursorEncoder.Encode(new Cursor<TId>(pagePaginables[^1].CreatedAt, pagePaginables[^1].Id))
                : null,
            await context.HasPreviousAsync(pagePaginables)
                ? cursorEncoder.Encode(new Cursor<TId>(pagePaginables[0].CreatedAt, pagePaginables[0].Id))
                : null);

        return new Page<TPaginable>(pagePaginables.AsReadOnly(), pageInfo);
    }
}