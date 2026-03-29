using Beatport2Rss.Application.Querying.Paging;
using Beatport2Rss.WebApi.Endpoints;

namespace Beatport2Rss.WebApi.Extensions;

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