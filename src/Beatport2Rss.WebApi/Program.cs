using Asp.Versioning;

using Beatport2Rss.Infrastructure;
using Beatport2Rss.WebApi;
using Beatport2Rss.WebApi.Constants;
using Beatport2Rss.WebApi.Endpoints;
using Beatport2Rss.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOpenApi()
    .AddApiVersioning(o =>
    {
        o.DefaultApiVersion = ApiVersionsContainer.Default;
        o.AssumeDefaultVersionWhenUnspecified = true;
        o.ReportApiVersions = true;
        o.ApiVersionReader = new HeaderApiVersionReader(ResponseHeaderNames.ApiVersion);
    });
builder.AddInfrastructure();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection()
    .AddTraceIdHandlingMiddleware()
    .AddExceptionsHandlingMiddleware();

var v1Builder = app
    .NewVersionedApi("Beatport2Rss API V1")
    .HasApiVersion(ApiVersionsContainer.V1);

v1Builder.MapGet("", () => "Hello, Beatport2Rss!");
v1Builder
    .BuildSessionEndpoints()
    .BuildUserEndpoints();

app.Run();