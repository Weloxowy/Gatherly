namespace gatherly.server.Entities;

public class NewUserDTO
{
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
    public virtual string AvatarName { get; set; }
    public virtual DateTime? LastTimeLogged { get; init; } 
}