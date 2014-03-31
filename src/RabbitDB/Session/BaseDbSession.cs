using RabbitDB.Query;
using RabbitDB.Reader;
using RabbitDB.Schema;
using RabbitDB.Session;
using RabbitDB.Storage;
using System;
using System.Data;

namespace RabbitDB.Base
{
    public abstract class BaseDbSession : IBaseDbSession, IDisposable
    {
        public BaseDbSession(string connectionString, DbEngine dbEngine)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString", "The connectionString cannot be null or empty!");

            _dbEngine = dbEngine;
            Initialize(connectionString, _dbEngine);
        }

        public BaseDbSession(Type assemblyType)
        {
            string connectionString = Registrar<string>.GetFor(assemblyType);
            _dbEngine = Registrar<DbEngine>.GetFor(assemblyType);
            Initialize(connectionString, _dbEngine);
        }

        public BaseDbSession(string connectionString)
            : this(connectionString, DbEngine.SqlServer) { }

        private void Initialize(string connectionString, DbEngine dbEngine)
        {
            _sqlDialect = SqlDialectFactory.Create(_dbEngine, connectionString);
            DbProviderAccessor.SqlDialect = _sqlDialect;
        }

        internal SqlDialect.SqlDialect _sqlDialect = null;
        protected DbEngine _dbEngine;

        private IDbPersister _dbPersister;
        internal IDbPersister DbPersister
        {
            get
            {
                if (_sqlDialect == null)
                {
                    throw new InvalidOperationException("SqlDialect is not initialized");
                }

                return _dbPersister ?? (_dbPersister = new DbPersister(_sqlDialect));
            }
        }

        EntitySet<TEntity> IBaseDbSession.GetEntitySet<TEntity>(IQuery query)
        {
            EntitySet<TEntity> objectSet = new EntitySet<TEntity>();
            return objectSet.Load(this, query);
        }

        EntityReader<TEntity> IBaseDbSession.GetEntityReader<TEntity>(IQuery query)
        {
            return _sqlDialect.ExecuteReader<TEntity>(query);
        }

        public IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            ITransactionalDbProvider transactionalProvider = _sqlDialect.DbProvider as ITransactionalDbProvider;
            if (transactionalProvider == null)
                throw new NotSupportedException(
                    string.Format(@"This type of DbEngine ('{0}') does not implement the interface ITransactionalDbProvider
                                  and so does not support transactional behavior.", _dbEngine));

            return isolationLevel.HasValue
                ? transactionalProvider.BeginTransaction(isolationLevel.Value)
                : transactionalProvider.BeginTransaction();
        }

        #region IDisposable

        internal void Dispose()
        {
            if (_dbPersister != null)
            {
                _dbPersister = null;
            }

            _sqlDialect.Dispose();
            DbSchemaAllocator.SchemaReader.Dispose();
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }

        #endregion
    }
}