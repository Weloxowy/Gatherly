using FluentMigrator;

namespace gatherly.server.Persistence.Meetings.Meeting._database;

[Migration(006)]
public class _006_CreateTable : Migration
{
    private readonly string _tableName = nameof(Models.Meetings.Meeting.Meeting);

    public override void Up()
    {
        if (!Schema.Table(_tableName).Exists())
        {
            Create.Table(_tableName)
                .WithColumn(nameof(Models.Meetings.Meeting.Meeting.Id)).AsGuid().NotNullable().PrimaryKey()
                .WithColumn(nameof(Models.Meetings.Meeting.Meeting.OwnerId)).AsGuid().NotNullable()
                .WithColumn(nameof(Models.Meetings.Meeting.Meeting.MeetingName)).AsString().NotNullable()
                .WithColumn(nameof(Models.Meetings.Meeting.Meeting.Description)).AsString().NotNullable()
                .WithColumn(nameof(Models.Meetings.Meeting.Meeting.PlaceName)).AsString().NotNullable()
                .WithColumn(nameof(Models.Meetings.Meeting.Meeting.Lon)).AsDouble().NotNullable()
                .WithColumn(nameof(Models.Meetings.Meeting.Meeting.Lat)).AsDouble().NotNullable()
                .WithColumn(nameof(Models.Meetings.Meeting.Meeting.StartOfTheMeeting)).AsDateTime().NotNullable()
                .WithColumn(nameof(Models.Meetings.Meeting.Meeting.EndOfTheMeeting)).AsDateTime().NotNullable()
                .WithColumn(nameof(Models.Meetings.Meeting.Meeting.IsMeetingTimePlanned)).AsBoolean().NotNullable()
                .WithColumn(nameof(Models.Meetings.Meeting.Meeting.TimeZone)).AsString().NotNullable();
        }
    }

    public override void Down()
    {
        Delete.Table(_tableName);    
    }
}