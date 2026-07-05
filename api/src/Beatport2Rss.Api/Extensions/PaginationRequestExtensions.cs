using Beatport2Rss.Api.Application.Querying.Paging;
using Beatport2Rss.Api.Endpoints;

namespace Beatport2Rss.Api.Extensions;

internal static class PaginationRequestExtensions
{
    extension(PaginationRequest request)
    {
        public Pagination ToPagination() =>
            new(request.Size,
                request.Next,
                request.Previous);
    }
}