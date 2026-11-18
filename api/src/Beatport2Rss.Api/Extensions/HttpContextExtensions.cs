using Beatport2Rss.Common.FluentResults.Constants;
using Beatport2Rss.Common.FluentResults.Extensions;

using FluentResults;

namespace Beatport2Rss.Api.Extensions;

internal static class HttpContextExtensions
{
    extension(HttpContext context)
    {
        public void SetLastModified(IResultBase result)
        {
            if (result.TryGetSuccessMetadata(MetadataKeys.LastModified, out var lastModifiedAsObject) &&
                lastModifiedAsObject is DateTimeOffset lastModified)
            {
                context.Response.GetTypedHeaders().LastModified = lastModified;
            }
        }
    }
}