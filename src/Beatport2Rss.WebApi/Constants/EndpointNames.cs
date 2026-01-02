namespace Beatport2Rss.WebApi.Constants;

internal static class HealthEndpointNames
{
    public const string Get = "GetHealth";
}

internal static class SessionEndpointNames
{
    public const string Create = "CreateSession";
    public const string DeleteAll = "DeleteAllSessions";
    public const string DeleteCurrent = "DeleteCurrentSession";
    public const string GetCurrent = "GetCurrentSession";
    public const string UpdateCurrent = "UpdateCurrentSession";
}

internal static class UserEndpointNames
{
    public const string Create = "CreateUser";
}