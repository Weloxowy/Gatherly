using FluentMigrator;

namespace gatherly.server.Persistence.Authentication.RecoverySession._database;
[Migration(005)]
public class _005_CreateTable : Migration
{
    private readonly string _tableName = "RecoverySession";
    
    public override void Up()
    {
        if (!Schema.Table(_tableName).Exists())
        {
            Create.Table(_tableName)
                .WithColumn(nameof(Models.Authentication.RecoverySession.RecoverySession.Id)).AsGuid().NotNullable()
                .PrimaryKey()
                .WithColumn(nameof(Models.Authentication.RecoverySession.RecoverySession.UserId)).AsGuid().NotNullable()
                .WithColumn(nameof(Models.Authentication.RecoverySession.RecoverySession.ExpiryDate)).AsDateTime()
                .WithColumn(nameof(Models.Authentication.RecoverySession.RecoverySession.IsOpened)).AsBoolean()
                .NotNullable();
            Create.ForeignKey("FK_Recovery_User").FromTable(_tableName).ForeignColumn("UserId").ToTable("UserEntity")
                .PrimaryColumn("Id");
        }
    }

    public override void Down()
    {
        if (!Schema.Table(_tableName).Exists()) return;
        Delete.ForeignKey("FK_Recovery_User");
        Delete.PrimaryKey(nameof(Models.Authentication.RecoverySession.RecoverySession.Id)).FromTable(_tableName);
        Delete.Table(_tableName);
    }
}