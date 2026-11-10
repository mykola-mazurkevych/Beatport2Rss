#pragma warning disable CA1034 // Nested types should not be visible

using System.Globalization;
using System.Net;

using Beatport2Rss.Common.FluentResults.Errors;

using FluentResults;

namespace Beatport2Rss.Common.FluentResults.Extensions;

public static class ResultExtensions
{
    extension(Result)
    {
        public static Result Conflict(string message) =>
            Result.Fail(new ConflictError(message));

        public static Result Fail(IResultBase result) =>
            Result.Fail(result.Errors);

        public static Result Forbidden(string message) =>
            Result.Fail(new ForbiddenError(message));

        public static Result NotFound(string message) =>
            Result.Fail(new NotFoundError(message));

        public static Result NotFound(string message, params object?[] args) =>
            Result.Fail(new NotFoundError(string.Format(CultureInfo.InvariantCulture, message, args)));

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
    }

    extension(IResultBase result)
    {
        public string ErrorMessage =>
            result.Errors.Count switch
            {
                0 => throw new InvalidOperationException(),
                1 => result.Errors.Single().Message,
                _ => $"Multiple errors occured:\n{string.Join('\n', result.Errors.Select(error => error.Message))}"
            };
    }
}