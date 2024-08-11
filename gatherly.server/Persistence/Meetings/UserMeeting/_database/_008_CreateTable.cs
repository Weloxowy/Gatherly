using FluentMigrator;

namespace gatherly.server.Persistence.Meetings.UserMeeting._database;

[Migration(008)]
public class _008_CreateTable : Migration
{
    private readonly string _tableName = nameof(Models.Meetings.UserMeeting.UserMeeting);

    public override void Up()
    {
        if (!Schema.Table(_tableName).Exists())
        {
            Create.Table(_tableName)
                .WithColumn(nameof(Models.Meetings.UserMeeting.UserMeeting.Id)).AsGuid().NotNullable().PrimaryKey()
                .WithColumn(nameof(Models.Meetings.UserMeeting.UserMeeting.UserId)).AsGuid().NotNullable()
                .WithColumn(nameof(Models.Meetings.UserMeeting.UserMeeting.MeetingId)).AsGuid().NotNullable()
                .WithColumn(nameof(Models.Meetings.UserMeeting.UserMeeting.Status)).AsInt32().NotNullable()
                .WithColumn(nameof(Models.Meetings.UserMeeting.UserMeeting.Availability)).AsBinary().NotNullable();
            Create.ForeignKey("FK_UserMeeting_User").FromTable(_tableName).ForeignColumn("UserId").ToTable("UserEntity")
                .PrimaryColumn("Id");
            Create.ForeignKey("FK_UserMeeting_Meeting").FromTable(_tableName).ForeignColumn("MeetingId").ToTable("Meeting")
                .PrimaryColumn("Id");
        }
    }

    public override void Down()
    {
        if (Schema.Table(_tableName).Exists())
        {
            Delete.ForeignKey("FK_UserMeeting_User");
            Delete.ForeignKey("FK_UserMeeting_Meeting");
            Delete.Table(_tableName);
        }
    }
}