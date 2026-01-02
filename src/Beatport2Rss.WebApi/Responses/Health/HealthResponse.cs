using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Beatport2Rss.WebApi.Responses.Health;

internal readonly record struct HealthResponse
{
    public required HealthStatus Status { get; init; }

    public required HealthDetailsResponse Details { get; init; }
}

internal readonly record struct HealthDetailsResponse
{
    public required HealthStatus DatabaseStatus { get; init; }
}