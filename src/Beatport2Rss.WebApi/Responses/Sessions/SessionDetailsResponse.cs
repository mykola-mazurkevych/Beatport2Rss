using Beatport2Rss.Application.ReadModels.Sessions;

namespace Beatport2Rss.WebApi.Responses.Sessions;

internal readonly record struct SessionDetailsResponse(
    Guid SessionId,
    DateTimeOffset CreatedAt,
    string EmailAddress,
    string? FirstName,
    string? LastName,
    string? UserAgent,
    string? IpAddress,
    bool IsExpired)
{
    public static SessionDetailsResponse Create(SessionDetailsReadModel readModel) =>
        new(
            readModel.SessionId,
            readModel.CreatedAt,
            readModel.EmailAddress,
            readModel.FirstName,
            readModel.LastName,
            readModel.UserAgent,
            readModel.IpAddress,
            readModel.IsExpired);
}