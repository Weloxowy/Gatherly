using FluentNHibernate.Mapping;

namespace gatherly.server.Models.Authentication.UserEntity;

public class UserEntityMapping : ClassMap<UserEntity>
{
    public UserEntityMapping()
    {
        Table("UserEntity");
        Id(x => x.Id);
        Map(x => x.Name);
        Map(x => x.Email);
        Map(x => x.PasswordHash);
        Map(x => x.LastTimeLogged);
        Map(x => x.AvatarName);
        Map(x => x.UserRole).CustomType<UserRole>();
    }
}