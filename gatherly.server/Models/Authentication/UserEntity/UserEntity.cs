namespace gatherly.server.Models.Authentication.UserEntity;

public class UserEntity
{
    public UserEntity() : base()
    {
    }
    
    public UserEntity(Guid id, string name, string email, string avatarName, DateTime? lastTimeLogged, UserRole userRole, string passwordHash)
    {
        Id = id;
        Name = name;
        Email = email;
        AvatarName = avatarName;
        LastTimeLogged = lastTimeLogged;
        UserRole = userRole;
        PasswordHash = passwordHash;
    }

    public virtual Guid Id { get; set; }
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
    public virtual string PasswordHash { get; set; }
    public virtual string AvatarName { get; set; }
    public virtual DateTime? LastTimeLogged { get; set; }
    public virtual UserRole UserRole { get; set; }
}