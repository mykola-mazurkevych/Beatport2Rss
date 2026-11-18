#pragma warning disable CA1034 // Nested types should not be visible

using System.Globalization;
using System.Net;

using Beatport2Rss.Common.FluentResults.Constants;
using Beatport2Rss.Common.FluentResults.Errors;
using Beatport2Rss.Common.FluentResults.Successes;

using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Extensions;

public static class ResultExtensions
{
    extension(Result result)
    {
        public static Result Conflict(string message) =>
            Result.Fail(new ConflictError(message));

        public static Result Fail(IResultBase resultBase) =>
            Result.Fail(resultBase.Errors);

        public static Result Forbidden(string message) =>
            Result.Fail(new ForbiddenError(message));

        public static Result NotFound(string message) =>
            Result.Fail(new NotFoundError(message));

        public static Result NotFound(string message, params object?[] args) =>
            Result.Fail(new NotFoundError(string.Format(CultureInfo.InvariantCulture, message, args)));

        public static Result NotModified() =>
            Result.Ok().WithSuccess<NotModifiedSuccess>();

        public static Result RequestTimeout(string message) =>
            Result.Fail(new RequestTimeoutError(message));

        public static Result TooManyRequests(string message) =>
            Result.Fail(new TooManyRequestsError(message));

        public static Result Transient(string message) =>
            Result.Fail(new TransientError(message));

        public static Result<TValue> Transient<TValue>(string message) =>
            Result.Fail<TValue>(new TransientError(message));

        public static Result Transient(string message, HttpStatusCode httpStatusCode) =>
            Result.Fail(new TransientHttpError(message, httpStatusCode));

        public static Result Validation(string message, Dictionary<string, object> metadata) =>
            Result.Fail(new ValidationError(message).WithMetadata(metadata));

        public static Result Unauthorized(string message) =>
            Result.Fail(new UnauthorizedError(message));

        public static Result Unprocessable(string message) =>
            Result.Fail(new UnprocessableError(message));

        public Result WithLastModified(DateTimeOffset lastModified)
        {
            if (result.Successes.Count == 0)
            {
                result.WithSuccess<SimpleSuccess>();
            }

            result.Successes.Single().Metadata[MetadataKeys.LastModified] = lastModified;
            return result;
        }
    }

    extension<T>(Result<T> result)
    {
        public Result<T> WithLastModified(DateTimeOffset lastModified)
        {
            if (result.Successes.Count == 0)
            {
                result.WithSuccess<SimpleSuccess>();
            }

            result.Successes.Single().Metadata[MetadataKeys.LastModified] = lastModified;
            return result;
        }
    }
}