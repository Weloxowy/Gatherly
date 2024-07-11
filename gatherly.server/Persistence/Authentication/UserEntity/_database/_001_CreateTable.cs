using FluentMigrator;

namespace gatherly.server.Persistence.Authentication.UserEntity._database;

[Migration(001)]
public class _001_CreateTable : Migration
{
    readonly string _tableName = "UserEntity";
    
    public override void Up()
    {
        if (!Schema.Table(_tableName).Exists())
        {
            Create.Table(_tableName)
                .WithColumn(nameof(Models.Authentication.UserEntity.UserEntity.Id)).AsGuid().NotNullable().PrimaryKey()
                .WithColumn(nameof(Models.Authentication.UserEntity.UserEntity.Name)).AsString().NotNullable()
                .WithColumn(nameof(Models.Authentication.UserEntity.UserEntity.Email)).AsString().NotNullable().Unique()
                .WithColumn(nameof(Models.Authentication.UserEntity.UserEntity.PasswordHash)).AsString().NotNullable()
                .WithColumn(nameof(Models.Authentication.UserEntity.UserEntity.AvatarName)).AsString().NotNullable()
                .WithColumn(nameof(Models.Authentication.UserEntity.UserEntity.LastTimeLogged)).AsDateTime().Nullable()
                .WithColumn(nameof(Models.Authentication.UserEntity.UserEntity.UserRole)).AsInt32().NotNullable();
        }
    }

    public override void Down()
    {
        if (!Schema.Table(_tableName).Exists()) return;
        Delete.PrimaryKey(nameof(Models.Authentication.UserEntity.UserEntity.Id)).FromTable(_tableName);
        Delete.Table(_tableName);
    }
}