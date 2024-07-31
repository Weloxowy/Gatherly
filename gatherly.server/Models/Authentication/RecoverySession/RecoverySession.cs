namespace gatherly.server.Models.Authentication.RecoverySession;

public class RecoverySession
{
    public RecoverySession() : base()
    {
    }

    public RecoverySession(Guid id, Guid userId, DateTime expiryDate, Boolean isOpened)
    {
        Id = id;
        UserId = userId;
        ExpiryDate = expiryDate;
        IsOpened = isOpened;
    }
    
    public virtual Guid Id { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual DateTime ExpiryDate  { get; set; }
    public virtual Boolean IsOpened  { get; set; }
    
}