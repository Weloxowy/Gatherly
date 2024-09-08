using FluentMigrator;

namespace gatherly.server.Persistence.Chat._database;

[Migration(009)]
public class _009_CreateTable_Message : Migration
{
    private readonly string _tableName = nameof(Models.Chat.Chat.Message);
    
    public override void Up()
    {
        if (!Schema.Table(_tableName).Exists())
        {
            Create.Table(_tableName)
                .WithColumn(nameof(Models.Chat.Chat.Message.Id)).AsGuid().NotNullable().PrimaryKey()
                .WithColumn(nameof(Models.Chat.Chat.Message.Content)).AsString().NotNullable()
                .WithColumn(nameof(Models.Chat.Chat.Message.Timestamp)).AsDateTime().NotNullable()
                .WithColumn(nameof(Models.Chat.Chat.Message.SenderId)).AsGuid().NotNullable()
                .WithColumn(nameof(Models.Chat.Chat.Message.MeetingId)).AsGuid().NotNullable();
            Create.ForeignKey("FK_Message_Sender")
                .FromTable("Message").ForeignColumn("SenderId")
                .ToTable("UserEntity").PrimaryColumn("Id");
            Create.ForeignKey("FK_Message_Meeting")
                .FromTable("Message").ForeignColumn("MeetingId")
                .ToTable("Meeting").PrimaryColumn("Id");
        }
    }

    public override void Down()
    {
        Delete.Table(_tableName);    
    }
}