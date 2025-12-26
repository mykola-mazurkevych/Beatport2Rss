#pragma warning disable CA1031 // Do not catch general exception types

using System.Net;

using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.WebApi.Responses;

using FluentValidation;

namespace Beatport2Rss.WebApi.Middlewares;

internal static class ExceptionsHandlingMiddleware
{
    extension(WebApplication app)
    {
        public IApplicationBuilder AddExceptionsHandlingMiddleware() => app.Use(HandleAsync);

        private static async Task HandleAsync(HttpContext context, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (ConflictException exception)
            {
                var response = new ConflictResponse
                {
                    Title = exception.Title,
                    Status = HttpStatusCode.Conflict,
                    Detail = exception.Detail
                };

                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (ValidationException exception)
            {
                var response = new BadRequestResponse
                {
                    Title = "One or more validation errors occurred",
                    Status = HttpStatusCode.BadRequest,
                    Detail = "The request contains invalid data.",
                    Errors = exception.Errors
                        .GroupBy(f => f.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(f => f.ErrorMessage))
                        .AsReadOnly()
                };

                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (Exception exception)
            {
                var response = new InternalServerErrorResponse
                {
                    Title = "Internal server error",
                    Status = HttpStatusCode.InternalServerError,
                    Detail = exception.Message
                };

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}