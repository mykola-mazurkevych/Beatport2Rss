using FluentResults;

namespace Beatport2Rss.WebApi.Extensions;

internal static class ResultExtensions
{
    extension(Result result)
    {
        public IResult ToIResult(Func<IResult> onSuccess, HttpContext context) =>
            result switch
            {
                { IsSuccess: true } or { IsFailed: false } => onSuccess(),
                { IsFailed: true } or { IsSuccess: false } => ProblemDetailsBuilder.Build(context, result.Errors)
            };
    }

    extension<T>(Result<T> result)
    {
        public IResult ToIResult(Func<IResult> onSuccess, HttpContext context) =>
            result switch
            {
                { IsSuccess: true } or { IsFailed: false } => onSuccess(),
                { IsFailed: true } or { IsSuccess: false } => ProblemDetailsBuilder.Build(context, result.Errors)
            };
    }
}