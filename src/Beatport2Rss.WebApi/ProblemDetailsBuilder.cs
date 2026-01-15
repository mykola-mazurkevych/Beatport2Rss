using System.Net;

using Beatport2Rss.SharedKernel.Errors;
using Beatport2Rss.WebApi.Constants;

using FluentResults;

namespace Beatport2Rss.WebApi;

internal static class ProblemDetailsBuilder
{
    public static IResult Build(HttpContext context, IEnumerable<IError> errors) =>
        errors.Single() switch
        {
            ConflictError conflict => Conflict(context, conflict.Message),
            ForbiddenError forbidden => Forbidden(context, forbidden.Message),
            NotFoundError notFound => NotFound(context, notFound.Message),
            ValidationError validation => BadRequest(context, validation.Message, validation.Metadata),
            _ => InternalServerError(context, "asd")
        };

    public static IResult BadRequest(HttpContext context, string detail, Dictionary<string, object> errors) =>
        Results.ValidationProblem(
            errors: errors.ToDictionary(kvp => kvp.Key, kvp => (string[])kvp.Value),
            detail: detail,
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.BadRequest,
            title: "Bad Request",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.400",
            extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } });

    public static IResult Unauthorized(HttpContext context, string detail) =>
        Results.Problem(
            detail: detail,
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.Unauthorized,
            title: "Unauthorized",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.401",
            extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } });

    public static IResult Forbidden(HttpContext context, string detail) =>
        Results.Problem(
            detail: detail,
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.Forbidden,
            title: "Forbidden",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.403",
            extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } });

    public static IResult NotFound(HttpContext context, string detail) =>
        Results.Problem(
            detail: detail,
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.NotFound,
            title: "Not Found",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.404",
            extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } });

    public static IResult Conflict(HttpContext context, string detail) =>
        Results.Problem(
            detail: detail,
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.Conflict,
            title: "Conflict",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.409",
            extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } });

    public static IResult InternalServerError(HttpContext context, string detail) =>
        Results.Problem(
            detail: detail,
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.InternalServerError,
            title: "Internal Server Error",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.500",
            extensions: new Dictionary<string, object?> { { ResponseExtensionNames.TraceId, context.TraceIdentifier } });
}