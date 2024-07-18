using gatherly.server.Models.Authentication.UserEntity;

namespace gatherly.server.Entities.Authentication;

public class UserEntityDTOResponse
{
    public UserEntityDTOResponse(string? name, string? email, string? avatarName, UserRole? userRole)
    {
        Name = name;
        Email = email;
        AvatarName = avatarName;
        UserRole = userRole;
    }

    public virtual string? Name { get; set; }
    public virtual string? Email { get; set; }
    public virtual string? AvatarName { get; set; }
    public virtual UserRole? UserRole { get; set; }
}

public static class UserEntityDTOResponseMapping
{
    public static UserEntityDTOResponse ToDto(this UserEntity userEntity)
    {
        return new UserEntityDTOResponse(
            userEntity.Name,
            userEntity.Email,
            userEntity.AvatarName,
            userEntity.UserRole
        );
    }
}