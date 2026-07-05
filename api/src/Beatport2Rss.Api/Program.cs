using Asp.Versioning;

using Beatport2Rss.Api;
using Beatport2Rss.Api.Application;
using Beatport2Rss.Api.Constants;
using Beatport2Rss.Api.Endpoints.Feeds;
using Beatport2Rss.Api.Endpoints.Sessions;
using Beatport2Rss.Api.Endpoints.Subscriptions;
using Beatport2Rss.Api.Endpoints.Tags;
using Beatport2Rss.Api.Endpoints.Users;
using Beatport2Rss.Api.Infrastructure;
using Beatport2Rss.Api.Middlewares;
using Beatport2Rss.Api.Transformers;
using Beatport2Rss.Api.Jobs;

using Microsoft.AspNetCore.HttpOverrides;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi(options => options.AddDocumentTransformer<BearerSecuritySchemeTransformer>())
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddJobs()
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
        options.DefaultHttpClient = new KeyValuePair<ScalarTarget, ScalarClient>(ScalarTarget.CSharp, ScalarClient.HttpClient);
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
    .BuildSubscriptionEndpoints(versionSet)
    .BuildTagEndpoints(versionSet)
    .BuildUserEndpoints(versionSet);

app.MapGet("", () => "Hello, Beatport2Rss!");

app.Run();