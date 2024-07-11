using FluentNHibernate.Mapping;

namespace gatherly.server.Models.Tokens.BlacklistToken;

public class BlacklistTokenMapping : ClassMap<BlacklistToken>
{
    public BlacklistTokenMapping()
    {
        Table("BlacklistToken");
        Id(x => x.Token);
        Map(x => x.EndOfBlacklisting);
        Map(x => x.UserId);
    }
}