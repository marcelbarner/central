using Central.Domain.Users;
using Central.Server.Features.Authentication;

using Riok.Mapperly.Abstractions;

namespace Central.Server.Mappers;

[Mapper]
public static partial class UserMapper
{
    [MapProperty(nameof(User.UserName), nameof(CurrentUserResponse.Username))]
    public static partial CurrentUserResponse ToCurrentUserResponse(this User user, List<string> roles);
}