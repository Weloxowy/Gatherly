namespace gatherly.server.Models.Authentication.SsoSession;

public class SsoSession
{
    public SsoSession()
    {
    }

    public SsoSession(Guid id, Guid? userId, string verificationCode, DateTime createdAt, DateTime expiresAt,
        string userEmail)
    {
        Id = id;
        UserId = userId;
        VerificationCode = verificationCode;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
        UserEmail = userEmail;
    }

    public virtual Guid Id { get; set; }
    public virtual Guid? UserId { get; set; }
    public virtual string UserEmail { get; set; }
    public virtual string VerificationCode { get; set; }
    public virtual DateTime CreatedAt { get; set; }
    public virtual DateTime ExpiresAt { get; set; }
}