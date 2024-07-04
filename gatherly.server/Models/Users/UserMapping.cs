using FluentNHibernate.Mapping;

namespace gatherly.server.Models.Users;

public class UserMapping : ClassMap<Users>
{
    public UserMapping()
    {
        Table("Users");
        Id(x => x.Id);
        Map(x => x.Name);
        Map(x => x.Email);
        Map(x => x.LastTimeLogged);
        Map(x => x.AvatarName);
    }
}