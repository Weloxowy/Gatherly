using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace gatherly.server;

public class NHibernateHelper
{
    private static ISessionFactory _sessionFactory;

    public static NHibernate.ISession OpenSession()
    {
        return SessionFactory.OpenSession();
    }

    private static ISessionFactory SessionFactory
    {
        get
        {
            if (_sessionFactory == null)
            {
                _sessionFactory = Fluently.Configure()
                    .Database(
                        MsSqlConfiguration.MsSql2012.ConnectionString(
                            "Server=localhost\\SQLEXPRESS;Database=Gatherly;Integrated Security=SSPI;Application Name=Gatherly;TrustServerCertificate=true;")
                    ) 
                    .Mappings(m =>
                        m.FluentMappings.AddFromAssemblyOf<Models.Users.Users>())
                    .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                    .BuildSessionFactory();


            }

            return _sessionFactory;
        }
    }
}