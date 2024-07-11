namespace gatherly.server.Entities.Tokens;

public class TokensDTOResponse
{
    public TokensDTOResponse(string jwt, string refresh)
    {
        JWT = jwt;
        Refresh = refresh;
    }

    public virtual string JWT { get; set; }
    public virtual string Refresh { get; set; }
}
