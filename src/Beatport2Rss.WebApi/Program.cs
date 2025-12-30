using Asp.Versioning;

using Beatport2Rss.Infrastructure;
using Beatport2Rss.WebApi;
using Beatport2Rss.WebApi.Constants;
using Beatport2Rss.WebApi.Endpoints;
using Beatport2Rss.WebApi.Middlewares;

using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddProblemDetails(o => o.CustomizeProblemDetails = context => context.ProblemDetails.Extensions[ExtensionNames.TraceId] = context.HttpContext.TraceIdentifier)
    .AddApiVersioning(o =>
    {
        o.DefaultApiVersion = ApiVersionsContainer.Default;
        o.AssumeDefaultVersionWhenUnspecified = true;
        o.ReportApiVersions = true;
        o.ApiVersionReader = new HeaderApiVersionReader(ResponseHeaderNames.ApiVersion);
    });
builder.AddInfrastructure();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

app.UseAuthentication();
app.UseAuthorization();

app.UseExceptionHandler()
    .UseHttpsRedirection()
    .AddTraceIdHandlingMiddleware();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var v1Builder = app
    .NewVersionedApi("Beatport2Rss API V1")
    .HasApiVersion(ApiVersionsContainer.V1);

v1Builder.MapGet("", () => "Hello, Beatport2Rss!");
v1Builder
    .BuildSessionEndpoints()
    .BuildUserEndpoints();

app.Run();