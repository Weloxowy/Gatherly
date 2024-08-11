using FluentMigrator;

namespace gatherly.server.Persistence.Tokens.RefreshToken._database;

[Migration(003)]
public class _003_CreateTable : Migration
{
    private readonly string _tableName = "RefreshToken";

    public override void Up()
    {
        if (!Schema.Table(_tableName).Exists())
        {
            Create.Table(_tableName)
                .WithColumn(nameof(Models.Tokens.RefreshToken.RefreshToken.Id)).AsGuid().NotNullable().PrimaryKey()
                .WithColumn(nameof(Models.Tokens.RefreshToken.RefreshToken.Token)).AsString().NotNullable()
                .WithColumn(nameof(Models.Tokens.RefreshToken.RefreshToken.UserId)).AsGuid().NotNullable()
                .WithColumn(nameof(Models.Tokens.RefreshToken.RefreshToken.IsRevoked)).AsBoolean().NotNullable()
                .WithColumn(nameof(Models.Tokens.RefreshToken.RefreshToken.Expiration)).AsDateTime().Nullable();
        }
    }

    public override void Down()
    {
        Delete.Table(_tableName);
    }
}