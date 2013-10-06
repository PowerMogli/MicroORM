using System;
using System.Data;

namespace MicroORM.Base
{
    public class DatabaseTransaction : IDbTransaction
    {
        [ThreadStatic]
        private static IsolationLevel _isolationLevel;
        [ThreadStatic]
        private static bool _isActive = false;
        [ThreadStatic]
        private static IDbTransaction _transaction;

        public DatabaseTransaction(IsolationLevel isolationLevel)
        {
            _isolationLevel = isolationLevel;
            _isActive = true;
        }

        internal static IDbTransaction Transaction { get { return _transaction; } }

        internal static bool IsActive { get { return _isActive; } }

        internal static void Create(IDbConnection connection)
        {
            if (!_isActive || _transaction != null) return;

            _transaction = connection.BeginTransaction(_isolationLevel);
        }

        public void Commit()
        {
            if (_transaction != null)
                _transaction.Commit();
        }

        public IDbConnection Connection
        {
            get { return _transaction.Connection; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return _isolationLevel; }
        }

        public void Rollback()
        {
            if (_transaction != null)
                _transaction.Rollback();
        }

        public void Dispose()
        {
            if (_transaction != null)
                _transaction.Dispose();

            _isActive = false;
        }
    }
}
