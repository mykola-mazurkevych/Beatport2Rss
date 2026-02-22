using Asp.Versioning;

using Beatport2Rss.Application;
using Beatport2Rss.Infrastructure;
using Beatport2Rss.WebApi;
using Beatport2Rss.WebApi.Constants;
using Beatport2Rss.WebApi.Endpoints.Feeds;
using Beatport2Rss.WebApi.Endpoints.Sessions;
using Beatport2Rss.WebApi.Endpoints.Tags;
using Beatport2Rss.WebApi.Endpoints.Users;
using Beatport2Rss.WebApi.Middlewares;
using Beatport2Rss.WebApi.Transformers;

using Microsoft.AspNetCore.HttpOverrides;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>())
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddProblemDetails(options => options.CustomizeProblemDetails = context => context.ProblemDetails.Extensions[ResponseExtensionNames.TraceId] = context.HttpContext.TraceIdentifier)
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = ApiVersionsContainer.Default;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
        options.ApiVersionReader = new HeaderApiVersionReader(ResponseHeaderNames.ApiVersion);
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = false;
    });

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });

app
    .UseAuthentication()
    .UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Beatport2Rss API Reference";
        options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.JavaScript, ScalarClient.HttpClient);
        options.AddPreferredSecuritySchemes("Bearer");
        options.PersistentAuthentication = true;
    });
    app.UseDeveloperExceptionPage();
}

app
    .UseExceptionHandler()
    .UseHttpsRedirection()
    .UseTraceIdMiddleware()
    .UseHealthCheckMiddleware();

var versionSet = app.NewApiVersionSet()
    .HasApiVersion(ApiVersionsContainer.V1)
    ////.HasApiVersion(ApiVersionsContainer.V2)
    .ReportApiVersions()
    .Build();

app
    .BuildFeedEndpoints(versionSet)
    .BuildSessionEndpoints(versionSet)
    .BuildTagEndpoints(versionSet)
    .BuildUserEndpoints(versionSet);

app.MapGet("", () => "Hello, Beatport2Rss!");

app.Run();