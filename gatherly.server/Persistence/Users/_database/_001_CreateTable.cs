using FluentMigrator;

namespace gatherly.server.Persistence.Users._database;

[Migration(001)]
public class _001_CreateTable : Migration
{
    public override void Up()
    {
        Create.Table("Users")
            .WithColumn(nameof(Models.Users.Users.Id)).AsGuid().NotNullable().PrimaryKey()
            .WithColumn(nameof(Models.Users.Users.Name)).AsString().NotNullable()
            .WithColumn(nameof(Models.Users.Users.Email)).AsString().NotNullable().Unique()
            .WithColumn(nameof(Models.Users.Users.AvatarName)).AsString().NotNullable()
            .WithColumn(nameof(Models.Users.Users.LastTimeLogged)).AsDateTime().Nullable();
    }

    public override void Down()
    {
        Delete.PrimaryKey(nameof(Models.Users.Users.Id)).FromTable("Users");
        Delete.Table("Users");
    }
}