using FluentMigrator;

namespace gatherly.server.Persistence.Meetings.Invitations._database;

[Migration(007)]
public class _007_CreateTable : Migration
{
    private readonly string _tableName = "Invitations";

    public override void Up()
    {
        if (!Schema.Table(_tableName).Exists())
        {
            Create.Table(_tableName)
                .WithColumn(nameof(Models.Meetings.Invitations.Invitations.Id)).AsGuid().NotNullable().PrimaryKey()
                .WithColumn(nameof(Models.Meetings.Invitations.Invitations.UserId)).AsGuid().NotNullable()
                .WithColumn(nameof(Models.Meetings.Invitations.Invitations.MeetingId)).AsGuid().NotNullable()
                .WithColumn(nameof(Models.Meetings.Invitations.Invitations.ValidTime)).AsDateTime()
                .NotNullable(); // assuming ValidTime is DateTime
            Create.ForeignKey("FK_Invitations_User")
                .FromTable("Invitations").ForeignColumn("UserId")
                .ToTable("UserEntity").PrimaryColumn("Id");

            Create.ForeignKey("FK_Invitations_Meeting")
                .FromTable("Invitations").ForeignColumn("MeetingId")
                .ToTable("Meeting").PrimaryColumn("Id");
        }
    }

    public override void Down()
    {

            Delete.ForeignKey("FK_Invitations_User").OnTable(_tableName);
            Delete.ForeignKey("FK_Invitations_Meeting").OnTable(_tableName);
            Delete.Table(_tableName);
    }
}