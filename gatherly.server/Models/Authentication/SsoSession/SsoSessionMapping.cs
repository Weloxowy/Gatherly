using FluentNHibernate.Mapping;

namespace gatherly.server.Models.Authentication.SsoSession;

public class SsoSessionMapping : ClassMap<SsoSession>
{
    public SsoSessionMapping()
    {
        Table("SsoSession");
        Id(x => x.Id);
        Map(x => x.UserId);
        Map(x => x.UserEmail);
        Map(x => x.VerificationCode);
        Map(x => x.CreatedAt);
        Map(x => x.ExpiresAt);
    }
}