using Beatport2Rss.Api.Application.Dtos.Users;

namespace Beatport2Rss.Api.Endpoints.Users.Responses;

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