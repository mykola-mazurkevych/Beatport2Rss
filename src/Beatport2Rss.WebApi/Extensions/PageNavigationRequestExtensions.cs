using Beatport2Rss.Application.Pagination;
using Beatport2Rss.WebApi.Endpoints;

namespace Beatport2Rss.WebApi.Extensions;

internal static class PageNavigationRequestExtensions
{
    extension(PageNavigationRequest request)
    {
        public PageNavigation ToPageNavigation() =>
            new(request.PageSize,
                request.NextPage,
                request.PreviousPage);
    }
}