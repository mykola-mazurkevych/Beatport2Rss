using System.Net;

using Beatport2Rss.WebApi.Constants;

namespace Beatport2Rss.WebApi;

// TODO: think about one generic method
internal static class ProblemDetailsBuilder
{
    public static IResult BadRequest(HttpContext context, IDictionary<string, string[]> errors) =>
        Results.ValidationProblem(
            errors: errors,
            detail: "One or more validation errors occured.",
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.BadRequest,
            title: "Bad Request",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.400",
            extensions: new Dictionary<string, object?> { { ExtensionNames.TraceId, context.TraceIdentifier } });

    public static IResult Unauthorized(HttpContext context, string detail) =>
        Results.Problem(
            detail: detail,
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.Unauthorized,
            title: "Unauthorized",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.401",
            extensions: new Dictionary<string, object?> { { ExtensionNames.TraceId, context.TraceIdentifier } });

    public static IResult Forbidden(HttpContext context, string detail) =>
        Results.Problem(
            detail: detail,
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.Forbidden,
            title: "Forbidden",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.403",
            extensions: new Dictionary<string, object?> { { ExtensionNames.TraceId, context.TraceIdentifier } });

    public static IResult NotFound(HttpContext context, string detail) =>
        Results.Problem(
            detail: detail,
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.NotFound,
            title: "Not Found",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.404",
            extensions: new Dictionary<string, object?> { { ExtensionNames.TraceId, context.TraceIdentifier } });

    public static IResult Conflict(HttpContext context, string detail) =>
        Results.Problem(
            detail: detail,
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.Conflict,
            title: "Conflict",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.409",
            extensions: new Dictionary<string, object?> { { ExtensionNames.TraceId, context.TraceIdentifier } });

    public static IResult UnprocessableEntity(HttpContext context, string detail) =>
        Results.Problem(
            detail: detail,
            instance: context.Request.Path,
            statusCode: (int)HttpStatusCode.UnprocessableEntity,
            title: "Unprocessable Entity",
            type: "https://datatracker.ietf.org/doc/html/rfc9110#status.422",
            extensions: new Dictionary<string, object?> { { ExtensionNames.TraceId, context.TraceIdentifier } });
}