using System.Net.Mime;

using Asp.Versioning.Builder;

using Beatport2Rss.Api.Endpoints.EmailConfirmations.Handlers;
using Beatport2Rss.Api.Endpoints.EmailConfirmations.Responses;

using Microsoft.AspNetCore.Mvc;

namespace Beatport2Rss.Api.Endpoints.EmailConfirmations;

internal static class EmailConfirmationEndpoints
{
    extension(IEndpointRouteBuilder routeBuilder)
    {
        public IEndpointRouteBuilder BuildEmailConfirmationEndpoints(ApiVersionSet versionSet)
        {
            var groupBuilder = routeBuilder
                .MapGroup("email-confirmations")
                .AllowAnonymous()
                .WithApiVersionSet(versionSet)
                .HasApiVersion(ApiVersionsContainer.V1)
                .WithTags("Email Confirmations");

            groupBuilder
                .MapGet("/{id}", GetEmailConfirmationEndpointHandler.Handle)
                .WithName(EmailConfirmationEndpointNames.Get)
                .WithDescription("Get email confirmation by its id")
                .WithSummary("Get Details")
                .Produces<EmailConfirmationResponse>(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status404NotFound, MediaTypeNames.Application.Json)
                .Produces<ProblemDetails>(StatusCodes.Status500InternalServerError, MediaTypeNames.Application.Json);

            return routeBuilder;
        }
    }
}