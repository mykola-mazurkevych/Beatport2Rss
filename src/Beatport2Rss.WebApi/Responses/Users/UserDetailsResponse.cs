using Beatport2Rss.Application.ReadModels.Users;

namespace Beatport2Rss.WebApi.Responses.Users;

internal readonly record struct UserDetailsResponse(
    string EmailAddress,
    string? FirstName,
    string? LastName,
    bool IsActive,
    int FeedsCount,
    int TagsCount)
{
    public static UserDetailsResponse Create(UserDetailsReadModel readModel) =>
        new(
            readModel.EmailAddress,
            readModel.FirstName,
            readModel.LastName,
            readModel.IsActive,
            readModel.FeedsCount,
            readModel.TagsCount);
}