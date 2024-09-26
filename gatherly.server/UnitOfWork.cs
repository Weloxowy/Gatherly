using NHibernate;
using ISession = NHibernate.ISession;

namespace gatherly.server;

public class UnitOfWork : IUnitOfWork
{
        private readonly ISession _session;
        private ITransaction _transaction;

        public UnitOfWork(ISession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public void BeginTransaction()
        {
            if (_transaction != null && _transaction.IsActive)
            {
                throw new InvalidOperationException("Transaction already in progress");
            }
            _transaction = _session.BeginTransaction();
        }

        public void Commit()
        {
            try
            {
                _transaction?.Commit();
            }
            catch
            {
                _transaction?.Rollback();
                throw;
            }
            finally
            {
                _transaction?.Dispose();
                _transaction = null;
            }
        }

        public void Rollback()
        {
            _transaction?.Rollback();
            _transaction?.Dispose();
            _transaction = null;
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _session.Dispose();
        }
}