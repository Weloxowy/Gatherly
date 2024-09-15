using FluentMigrator;

namespace gatherly.server.Persistence.Meetings.UserMeeting._database
{
    [Migration(010)] 
    public class _010_AlterTable : Migration
    {
        private readonly string _tableName = nameof(Models.Authentication.UserEntity.UserEntity);

        public override void Up()
        {
            if (Schema.Table(_tableName).Exists())
            {
                Execute.Sql(@" IF NOT EXISTS (SELECT 1 FROM DBO.UserEntity WHERE Id = 'FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF')
                    BEGIN
                        INSERT INTO DBO.UserEntity (Id, Name, Email, PasswordHash, LastTimeLogged, AvatarName, UserRole) 
                        VALUES ('FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF', 'Admin', NULL, 'unreachable', NULL, NULL, NULL);
                    END");
                /*
                Execute.Sql(@"INSERT INTO DBO.UserEntity(Id, Name, Email, PasswordHash, LastTimeLogged, AvatarName, UserRole) 
                              VALUES ('FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF', 'Admin', NULL, 'unreachable', NULL, NULL, NULL);");
                              */
            }
        }

        public override void Down()
        {
            return;
        }
    }
}