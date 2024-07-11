using FluentMigrator;

namespace gatherly.server.Persistence.Authentication.SsoSession._database;

[Migration(002)]
public class _002_CreateTable : Migration
{
    readonly string _tableName = "SsoSession";

    public override void Up()
    {
        if (!Schema.Table(_tableName).Exists())
        {
            Create.Table(_tableName)
                .WithColumn(nameof(Models.Authentication.SsoSession.SsoSession.Id)).AsGuid().NotNullable().PrimaryKey()
                .WithColumn(nameof(Models.Authentication.SsoSession.SsoSession.UserId)).AsGuid().NotNullable()
                .WithColumn(nameof(Models.Authentication.SsoSession.SsoSession.UserEmail)).AsString().NotNullable()
                .WithColumn(nameof(Models.Authentication.SsoSession.SsoSession.IsVerified)).AsBoolean().NotNullable()
                .Unique()
                .WithColumn(nameof(Models.Authentication.SsoSession.SsoSession.CreatedAt)).AsDateTime().NotNullable()
                .WithColumn(nameof(Models.Authentication.SsoSession.SsoSession.ExpiresAt)).AsDateTime().Nullable()
                .WithColumn(nameof(Models.Authentication.SsoSession.SsoSession.VerificationCode)).AsString()
                .NotNullable();
            Create.ForeignKey("FK_SSO_User").FromTable(_tableName).ForeignColumn("UserId").ToTable("UserEntity")
                .PrimaryColumn("Id");
        }
    }

    public override void Down()
    {
        if (!Schema.Table(_tableName).Exists()) return;
        Delete.ForeignKey("FK_SSO_User");
        Delete.PrimaryKey(nameof(Models.Authentication.SsoSession.SsoSession.Id)).FromTable(_tableName);
        Delete.Table(_tableName);
    }
}