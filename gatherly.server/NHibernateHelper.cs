using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace gatherly.server
{
    public class NHibernateHelper
    {
        private static ISessionFactory _sessionFactory;

        public static ISessionFactory SessionFactory
        {
            get
            {
                if (_sessionFactory == null)
                {
                    _sessionFactory = Fluently.Configure()
                        .Database(
                            MsSqlConfiguration.MsSql2012.ConnectionString(
                                "Server=localhost\\SQLEXPRESS;Database=Gatherly;Integrated Security=SSPI;Application Name=Gatherly;TrustServerCertificate=true;MultipleActiveResultSets=True")
                        )
                        .Mappings(m =>
                            m.FluentMappings.AddFromAssemblyOf<Models.Authentication.UserEntity.UserEntity>())
                        .Mappings(m =>
                            m.FluentMappings.AddFromAssemblyOf<Models.Authentication.SsoSession.SsoSession>())
                        .Mappings(m =>
                            m.FluentMappings.AddFromAssemblyOf<Models.Tokens.RefreshToken.RefreshToken>())
                        .Mappings(m =>
                            m.FluentMappings.AddFromAssemblyOf<Models.Tokens.BlacklistToken.BlacklistToken>())
                        /*
                        .Mappings(m =>
                            m.FluentMappings.AddFromAssemblyOf<Models.Mailing.MailEntity.MailEntity>())
                        */
                        .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                        .BuildSessionFactory();
                }

                return _sessionFactory;
            }
        }

        public static NHibernate.ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
