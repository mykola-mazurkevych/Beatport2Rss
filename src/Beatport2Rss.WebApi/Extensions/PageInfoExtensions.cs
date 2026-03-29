using System.Globalization;

using Beatport2Rss.Application.Querying.Paging;
using Beatport2Rss.WebApi.Constants;

namespace Beatport2Rss.WebApi.Extensions;

internal static class PageInfoExtensions
{
    extension(PageInfo pageInfo)
    {
        public void ToHeaders(HttpContext context)
        {
            context.Response.Headers[ResponseHeaderNames.PageSize] = pageInfo.Size.ToString(CultureInfo.InvariantCulture);

            context.Response.Headers[ResponseHeaderNames.PageTotalCount] = pageInfo.TotalCount.ToString(CultureInfo.InvariantCulture);

            context.Response.Headers[ResponseHeaderNames.PageItemsCount] = pageInfo.ItemsCount.ToString(CultureInfo.InvariantCulture);
            context.Response.Headers[ResponseHeaderNames.PageTotalItemsCount] = pageInfo.TotalItemsCount.ToString(CultureInfo.InvariantCulture);

            context.Response.Headers[ResponseHeaderNames.PageNext] = pageInfo.Next;
            context.Response.Headers[ResponseHeaderNames.PagePrevious] = pageInfo.Previous;
        }
    }
}