using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

namespace Beatport2Rss.WebApi.Transformers;

internal sealed class BearerSecuritySchemeTransformer(
    IAuthenticationSchemeProvider authenticationSchemeProvider) :
    IOpenApiDocumentTransformer
{
    public async Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        var authenticationSchemes = await authenticationSchemeProvider.GetAllSchemesAsync();
        if (authenticationSchemes.All(a => a.Name != "Bearer"))
        {
            return;
        }

        var bearerScheme = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Authorization header using the Bearer scheme."
        };

        document.Components ??= new OpenApiComponents();

        document.AddComponent("Bearer", bearerScheme);

        var securityRequirement = new OpenApiSecurityRequirement { [new OpenApiSecuritySchemeReference("Bearer", document)] = [] };

        var operations = document.Paths.Values
            .Where(pathItem => pathItem.Operations is not null)
            .SelectMany(pathItem => pathItem.Operations!.Values);
        foreach (var operation in operations)
        {
            operation.Security ??= [];
            operation.Security.Add(securityRequirement);
        }
    }
}