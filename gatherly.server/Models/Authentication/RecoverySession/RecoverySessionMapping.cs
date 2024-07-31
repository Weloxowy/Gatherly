using FluentNHibernate.Mapping;

namespace gatherly.server.Models.Authentication.RecoverySession;

public class RecoverySessionMapping : ClassMap<RecoverySession>
{
    public RecoverySessionMapping()
    {
        Table("RecoverySession");
        Id(x => x.Id);
        Map(x => x.UserId);
        Map(x => x.ExpiryDate);
        Map(x => x.IsOpened);
    }
}