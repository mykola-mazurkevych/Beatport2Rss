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

    public async Task<Page<TPageDto>> BuildAsync<TPageDto, TId>(
        IQueryable<TPageDto> dtos,
        int? size,
        string? next,
        string? previous,
        CancellationToken cancellationToken = default)
        where TPageDto : class, IPageDto<TId>
        where TId : struct, IId<TId>
    {
        var page = new Page<TPageDto> { Size = size ?? DefaultSize, TotalCount = await dtos.CountAsync(cancellationToken) };

        if (page.TotalCount == 0)
        {
            return page;
        }

        var definition = PaginationQuery.Build<TPageDto>(builder => builder.Ascending(dto => dto.CreatedAt).Ascending(dto => dto.Id));

        var nextCursor = cursorEncoder.Decode<TId>(next);
        var previousCursor = cursorEncoder.Decode<TId>(previous);

        PaginationDirection direction;
        TPageDto? reference = null;

        if (previousCursor is null)
        {
            direction = PaginationDirection.Forward;
            if (nextCursor is not null)
            {
                reference = await dtos.FirstOrDefaultAsync(dto => dto.Id.Equals(nextCursor.Id), cancellationToken);
            }
        }
        else
        {
            direction = PaginationDirection.Backward;
            reference = await dtos.FirstOrDefaultAsync(dto => dto.Id.Equals(previousCursor.Id), cancellationToken);
        }

        var context = dtos.Paginate(definition, direction, reference);

        var pageDtos = await context.Query
            .Take(page.Size)
            .ToListAsync(cancellationToken);

        context.EnsureCorrectOrder(pageDtos);

        page = page with
        {
            Items = pageDtos.AsReadOnly(),
            Next = await context.HasNextAsync(pageDtos) ? cursorEncoder.Encode(new Cursor<TId>(pageDtos[^1].CreatedAt, pageDtos[^1].Id)) : null,
            Previous = await context.HasPreviousAsync(pageDtos) ? cursorEncoder.Encode(new Cursor<TId>(pageDtos[0].CreatedAt, pageDtos[0].Id)) : null,
        };

        return page;
    }
}