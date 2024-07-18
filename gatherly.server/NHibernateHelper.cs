using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using gatherly.server.Models.Authentication.SsoSession;
using gatherly.server.Models.Authentication.UserEntity;
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
                _sessionFactory = Fluently.Configure()
                    .Database(
                        MsSqlConfiguration.MsSql2012.ConnectionString(
                            "Server=localhost\\SQLEXPRESS;Database=Gatherly;Integrated Security=SSPI;Application Name=Gatherly;TrustServerCertificate=true;MultipleActiveResultSets=True")
                    )
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<UserEntity>())
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<SsoSession>())
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<RefreshToken>())
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<BlacklistToken>())
                    /*
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<Models.Mailing.MailEntity.MailEntity>())
                    */
                    .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(true, true))
                    .BuildSessionFactory();

            return _sessionFactory;
        }
    }

    public static ISession OpenSession()
    {
        return SessionFactory.OpenSession();
    }
}