using System;
using System.Data;
using MicroORM.Storage;

namespace MicroORM.Base
{
    internal class EntitySession : IEntitySession, IDisposable
    {
        private DbSession _dbSession;

        internal EntitySession(string connectionString, DbEngine dbEngine)
        {
            _dbSession = new DbSession(connectionString, dbEngine);
        }

        public void Load<TEntity>(TEntity entity) where TEntity : Entity
        {
            _dbSession.Load(entity);
        }

        public void Update<TEntity>(TEntity entity) where TEntity : Entity
        {
            _dbSession.Update(entity);
        }

        public void Insert<TEntity>(TEntity entity) where TEntity : Entity
        {
            _dbSession.Insert(entity);
        }

        void IDisposable.Dispose()
        {
            ((IDbSession)_dbSession).Dispose();
        }

        public IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            return _dbSession.BeginTransaction(isolationLevel);
        }
    }
}
