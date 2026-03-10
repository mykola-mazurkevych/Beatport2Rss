using Beatport2Rss.Application.Dtos.Users;

namespace Beatport2Rss.WebApi.Endpoints.Users.Responses;

internal sealed record UserResponse(
    string EmailAddress,
    string? FirstName,
    string? LastName,
    bool IsActive,
    int FeedsCount,
    int TagsCount)
{
    public static UserResponse Create(UserDto dto) =>
        new(dto.EmailAddress.Value,
            dto.FirstName,
            dto.LastName,
            dto.IsActive,
            dto.FeedsCount,
            dto.TagsCount);
}