using System.Globalization;

using Beatport2Rss.Application.Pagination;
using Beatport2Rss.WebApi.Constants;

namespace Beatport2Rss.WebApi.Extensions;

internal static class PageMetadataExtensions
{
    extension(PageMetadata metadata)
    {
        public void ToHeaders(HttpContext context)
        {
            context.Response.Headers[ResponseHeaderNames.PageSize] = metadata.Size.ToString(CultureInfo.InvariantCulture);

            context.Response.Headers[ResponseHeaderNames.PageTotalCount] = metadata.TotalCount.ToString(CultureInfo.InvariantCulture);

            context.Response.Headers[ResponseHeaderNames.PageItemsCount] = metadata.ItemsCount.ToString(CultureInfo.InvariantCulture);
            context.Response.Headers[ResponseHeaderNames.PageTotalItemsCount] = metadata.TotalItemsCount.ToString(CultureInfo.InvariantCulture);

            context.Response.Headers[ResponseHeaderNames.PageNext] = metadata.Next;
            context.Response.Headers[ResponseHeaderNames.PagePrevious] = metadata.Previous;
        }
    }
}