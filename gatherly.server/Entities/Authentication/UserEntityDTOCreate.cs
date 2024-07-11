namespace gatherly.server.Entities.Authentication;

public class UserEntityDTOCreate
{
    public virtual string Name { get; set; }
    public virtual string Email { get; set; }
    public virtual string Password { get; set; }

}