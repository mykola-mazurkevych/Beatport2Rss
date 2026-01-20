using FluentResults;

namespace Beatport2Rss.WebApi.Extensions;

internal static class ResultExtensions
{
    extension(Result result)
    {
        public IResult ToAspNetCoreResult(Func<IResult> onSuccess, HttpContext context) =>
            result switch
            {
                { IsSuccess: true } or { IsFailed: false } => onSuccess(),
                { IsFailed: true } or { IsSuccess: false } => result.Error.ToProblemDetails(context),
            };

        private IError Error => result.Errors.Single();
    }

    extension<T>(Result<T> result)
    {
        public IResult ToAspNetCoreResult(Func<IResult> onSuccess, HttpContext context) =>
            result switch
            {
                { IsSuccess: true } or { IsFailed: false } => onSuccess(),
                { IsFailed: true } or { IsSuccess: false } => result.Error.ToProblemDetails(context),
            };

        private IError Error => result.Errors.Single();
    }
}