using System.Net;

using Beatport2Rss.SharedKernel.Errors;
using Beatport2Rss.WebApi.Constants;

using FluentResults;

namespace Beatport2Rss.WebApi.Extensions;

internal static class ErrorExtensions
{
    extension(IError error)
    {
        public IResult ToProblemDetails(HttpContext context) =>
            error switch
            {
                ConflictError conflict => conflict.ToAspNetCoreResult(context),
                ForbiddenError forbidden => forbidden.ToAspNetCoreResult(context),
                NotFoundError notFound => notFound.ToAspNetCoreResult(context),
                ValidationError validation => validation.ToAspNetCoreResult(context),
                UnauthorizedError unauthorized => unauthorized.ToAspNetCoreResult(context),
                _ => Results.Problem(
                    detail: "Unhandled error",
                    instance: context.Request.Path,
                    statusCode: (int)HttpStatusCode.InternalServerError,
                    title: "Internal Server Error",
                    type: "https://datatracker.ietf.org/doc/html/rfc9110#status.500",
                    extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } })
            };
    }

    extension(ConflictError conflict)
    {
        private IResult ToAspNetCoreResult(HttpContext context) =>
            Results.Problem(
                detail: conflict.Message,
                instance: context.Request.Path,
                statusCode: (int)HttpStatusCode.Conflict,
                title: "Conflict",
                type: "https://datatracker.ietf.org/doc/html/rfc9110#status.409",
                extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } });
    }

    extension(ForbiddenError forbidden)
    {
        private IResult ToAspNetCoreResult(HttpContext context) =>
            Results.Problem(
                detail: forbidden.Message,
                instance: context.Request.Path,
                statusCode: (int)HttpStatusCode.Forbidden,
                title: "Forbidden",
                type: "https://datatracker.ietf.org/doc/html/rfc9110#status.403",
                extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } });
    }

    extension(NotFoundError notFound)
    {
        private IResult ToAspNetCoreResult(HttpContext context) =>
            Results.Problem(
                detail: notFound.Message,
                instance: context.Request.Path,
                statusCode: (int)HttpStatusCode.NotFound,
                title: "Not Found",
                type: "https://datatracker.ietf.org/doc/html/rfc9110#status.404",
                extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } });
    }

    extension(UnauthorizedError unauthorized)
    {
        private IResult ToAspNetCoreResult(HttpContext context) =>
            Results.Problem(
                detail: unauthorized.Message,
                instance: context.Request.Path,
                statusCode: (int)HttpStatusCode.Unauthorized,
                title: "Unauthorized",
                type: "https://datatracker.ietf.org/doc/html/rfc9110#status.401",
                extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } });
    }

    extension(ValidationError validation)
    {
        private IResult ToAspNetCoreResult(HttpContext context) =>
            Results.ValidationProblem(
                errors: validation.Metadata.ToDictionary(kvp => kvp.Key, kvp => (string[])kvp.Value),
                detail: validation.Message,
                instance: context.Request.Path,
                statusCode: (int)HttpStatusCode.BadRequest,
                title: "Bad Request",
                type: "https://datatracker.ietf.org/doc/html/rfc9110#status.400",
                extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } });
    }
}