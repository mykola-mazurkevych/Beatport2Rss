namespace Beatport2Rss.WebApi.Responses.Health;

public readonly record struct HealthResponse(bool DatabaseIsHealthy)
{
    public bool IsHealthy => DatabaseIsHealthy;
}