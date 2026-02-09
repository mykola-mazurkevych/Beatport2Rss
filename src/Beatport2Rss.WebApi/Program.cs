using Asp.Versioning;

using Beatport2Rss.Application;
using Beatport2Rss.Infrastructure;
using Beatport2Rss.WebApi;
using Beatport2Rss.WebApi.Constants;
using Beatport2Rss.WebApi.Endpoints;
using Beatport2Rss.WebApi.Extensions;
using Beatport2Rss.WebApi.Middlewares;

using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureHttpJsonOptions(options => options.SerializerOptions.AddJsonValueConverters())
    .AddOpenApi()
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
    .BuildUserEndpoints(versionSet);

app.MapGet("", () => "Hello, Beatport2Rss!");

app.Run();