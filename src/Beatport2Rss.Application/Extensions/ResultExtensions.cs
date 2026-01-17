using Beatport2Rss.SharedKernel.Errors;

using FluentResults;

namespace Beatport2Rss.Application.Extensions;

internal static class ResultExtensions
{
    extension(Result)
    {
        public static Result Conflict(string message) =>
            Result.Fail(new ConflictError(message));

        public static Result Forbidden(string message) =>
            Result.Fail(new ForbiddenError(message));

        public static Result NotFound(string message) =>
            Result.Fail(new NotFoundError(message));

        public static Result Validation(string message, Dictionary<string, object> metadata) =>
            Result.Fail(new ValidationError(message).WithMetadata(metadata));

        public static Result Unauthorized(string message) =>
            Result.Fail(new UnauthorizedError(message));
    }
}