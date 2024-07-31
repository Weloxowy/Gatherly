namespace gatherly.server.Entities.Authentication;

public class UserEntityDTOResetPassword
{
    public virtual string Email { get; set; }
    public virtual string NewPassword { get; set; }
}