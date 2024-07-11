using FluentMigrator;

namespace gatherly.server.Persistence.Tokens.BlacklistToken._database;

[Migration(004)]
public class _004_CreateTable : Migration
{
    readonly string _tableName = "BlacklistToken";
    
    public override void Up()
    {
        if (!Schema.Table(_tableName).Exists())
        {
            Create.Table(_tableName)
                .WithColumn(nameof(Models.Tokens.BlacklistToken.BlacklistToken.Token)).AsString().NotNullable()
                .PrimaryKey()
                .WithColumn(nameof(Models.Tokens.BlacklistToken.BlacklistToken.EndOfBlacklisting)).AsDateTime()
                .NotNullable()
                .WithColumn(nameof(Models.Tokens.BlacklistToken.BlacklistToken.UserId)).AsGuid().NotNullable();
        }
    }

    public override void Down()
    {
        if (!Schema.Table(_tableName).Exists()) return;
        Delete.PrimaryKey(nameof(Models.Tokens.BlacklistToken.BlacklistToken.Token)).FromTable(_tableName);
        Delete.Table(_tableName);
    }
}