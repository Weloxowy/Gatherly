using FluentNHibernate.Mapping;

namespace gatherly.server.Models.Tokens.RefreshToken;

public class RefreshTokenMapping : ClassMap<RefreshToken>
{
    public RefreshTokenMapping()
    {
        Table("RefreshToken");
        Id(x => x.Id);
        Map(x => x.Token);
        Map(x => x.Expiration);
        Map(x => x.IsRevoked);
        Map(x => x.UserId);
    }
}