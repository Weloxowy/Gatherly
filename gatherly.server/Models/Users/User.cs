namespace gatherly.server.Models.Users;

public class Users
{
    public virtual Guid Id { get; set; }
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
    public virtual string AvatarName { get; set; }
    public virtual DateTime? LastTimeLogged { get; set; }
    
}