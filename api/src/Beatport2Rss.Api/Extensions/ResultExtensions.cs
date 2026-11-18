using Beatport2Rss.Common.FluentResults.Extensions;

using FluentResults;

namespace Beatport2Rss.Api.Extensions;

internal static class ResultExtensions
{
    extension(Result result)
    {
        public IResult ToAspNetCoreResult(Func<IResult> onSuccess, HttpContext context)
        {
            context.SetLastModified(result);

            return result switch
            {
                { IsNotModified: true } => Results.StatusCode(StatusCodes.Status304NotModified),
                { IsSuccess: true } => onSuccess(),
                { IsFailed: true } => result.Error.ToProblemDetails(context),
                _ => throw new NotSupportedException("Result state is not supported"),
            };
        }
    }

    extension<T>(Result<T> result)
    {
        public IResult ToAspNetCoreResult(Func<IResult> onSuccess, HttpContext context)
        {
            context.SetLastModified(result);

            return result switch
            {
                { IsNotModified: true } => Results.StatusCode(StatusCodes.Status304NotModified),
                { IsSuccess: true } => onSuccess(),
                { IsFailed: true } => result.Error.ToProblemDetails(context),
                _ => throw new NotSupportedException("Result state is not supported"),
            };
        }

        public IResult ToAspNetCoreResult(Func<T, IResult> onSuccess, HttpContext context)
        {
            context.SetLastModified(result);

            return result switch
            {
                { IsNotModified: true } => Results.StatusCode(StatusCodes.Status304NotModified),
                { IsSuccess: true } => onSuccess(result.Value),
                { IsFailed: true } => result.Error.ToProblemDetails(context),
                _ => throw new NotSupportedException("Result state is not supported"),
            };
        }
    }
}