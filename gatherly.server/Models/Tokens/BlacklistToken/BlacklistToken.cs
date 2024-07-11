namespace gatherly.server.Models.Tokens.BlacklistToken;

public class BlacklistToken
{
    public BlacklistToken() : base() {}
    
    public BlacklistToken(string token, Guid userId, DateTime endOfBlacklisting)
    {
        Token = token;
        UserId = userId;
        EndOfBlacklisting = endOfBlacklisting;
    }
    
    public virtual string Token { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual DateTime EndOfBlacklisting { get; set; }
}