using Beatport2Rss.Application.Dtos.Users;

namespace Beatport2Rss.WebApi.Endpoints.Users.Responses;

public sealed record GetUserResponse(
    string EmailAddress,
    string? FirstName,
    string? LastName,
    bool IsActive,
    int FeedsCount,
    int TagsCount)
{
    public static GetUserResponse Create(UserDto dto) =>
        new(dto.EmailAddress,
            dto.FirstName,
            dto.LastName,
            dto.IsActive,
            dto.FeedsCount,
            dto.TagsCount);
}