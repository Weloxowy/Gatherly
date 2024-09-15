using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using gatherly.server.Models.Authentication.RecoverySession;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;
using gatherly.server.Models.Chat.Chat;
using gatherly.server.Models.Meetings.Invitations;
using gatherly.server.Models.Meetings.Meeting;
using gatherly.server.Models.Meetings.UserMeeting;
using gatherly.server.Models.Tokens.BlacklistToken;
using gatherly.server.Models.Tokens.RefreshToken;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using ISession = NHibernate.ISession;

namespace gatherly.server;

public class NHibernateHelper
{
    private static ISessionFactory _sessionFactory;

    public static ISessionFactory SessionFactory
    {
        get
        {
            if (_sessionFactory == null)
            {
                // Inicjalizacja na podstawie pliku konfiguracyjnego
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build();

                var connectionString = configuration.GetConnectionString("DefaultConnection");

                _sessionFactory = Fluently.Configure()
                    .Database(
                        MsSqlConfiguration.MsSql2012.ConnectionString(connectionString)
                    )
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<UserEntity>())
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<SsoSession>())
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<RefreshToken>())
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<BlacklistToken>())
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<RecoverySession>())
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<Message>())
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<Invitations>())
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<Meeting>())
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<UserMeeting>())
                    /*
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<Models.Mailing.MailEntity.MailEntity>())
                    */
                    .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(true, true))
                    .BuildSessionFactory();
            }

            return _sessionFactory;
        }
    }

    public static ISession OpenSession()
    {
        return SessionFactory.OpenSession();
    }
}