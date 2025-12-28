#pragma warning disable CA1031 // Do not catch general exception types

using System.Net;

using Beatport2Rss.Domain.Common.Exceptions;
using Beatport2Rss.WebApi.Responses;

using FluentValidation;

namespace Beatport2Rss.WebApi.Middlewares;

internal static class ExceptionsHandlingMiddleware
{
    extension(IApplicationBuilder app)
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
                    Detail = exception.Detail,
                    TraceId = context.TraceIdentifier
                };

                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                await context.Response.WriteAsJsonAsync(response);
            }
            catch(InactiveUserException exception)
            {
                var response = new ForbiddenResponse { Error = exception.Message };

                context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                await context.Response.WriteAsJsonAsync(response);
            }
            catch(InvalidCredentialsException exception)
            {
                var response = new UnauthorizedResponse { Error = exception.Message };

                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                await context.Response.WriteAsJsonAsync(response);
            }
            catch (ValidationException exception)
            {
                var response = new BadRequestResponse
                {
                    Title = "One or more validation errors occurred",
                    Status = HttpStatusCode.BadRequest,
                    Detail = "The request contains invalid data.",
                    TraceId = context.TraceIdentifier,
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
                    Detail = exception.Message,
                    TraceId = context.TraceIdentifier,
                };

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}