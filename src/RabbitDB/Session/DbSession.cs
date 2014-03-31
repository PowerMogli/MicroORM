using RabbitDB.Query;
using RabbitDB.Query.Generic;
using RabbitDB.Reader;
using RabbitDB.Storage;
using System;
using System.Linq.Expressions;

namespace RabbitDB.Base
{
    public class DbSession : ReadOnlySession, IDisposable, IDbSession
    {
        #region Ctor

        public DbSession(string connectionString, DbEngine dbEngine)
            : base(connectionString, dbEngine) { }

        public DbSession(Type assemblyType)
            : base(assemblyType) { }

        public DbSession(string connectionString)
            : this(connectionString, DbEngine.SqlServer) { }

        #endregion

        ~DbSession()
        {
            base.Dispose();
        }

        private DbEntityPersister _dbEntityPersister;
        internal DbEntityPersister DbEntityPersister
        {
            get
            {
                if (_sqlDialect == null)
                {
                    throw new InvalidOperationException("DbProvider is not initialized");
                }

                return _dbEntityPersister ?? (_dbEntityPersister = new DbEntityPersister(DbPersister));
            }
        }

        public static Configuration Configuration { get { return Configuration.Instance; } }

        public void ExecuteCommand(string sql, params object[] arguments)
        {
            _sqlDialect.ExecuteCommand(new SqlQuery(sql, QueryParameterCollection.Create(arguments)));
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> criteria, params object[] setArguments)
        {
            DbPersister.Update<TEntity>(new UpdateExpressionQuery<TEntity>(criteria, setArguments));
        }

        public void Update<TEntity>(TEntity entity)
        {
            DbPersister.Update<TEntity>(new UpdateQuery<TEntity>(entity));
        }

        void IDbSession.Load<TEntity>(TEntity entity)
        {
            EntityReader<TEntity> objectReader = (EntityReader<TEntity>)_sqlDialect.ExecuteReader<TEntity>(new EntityQuery<TEntity>(entity));
            if (objectReader.Load(entity) == false) throw new Exception("Loading data was not successfull!");
        }

        public void Delete<TEntity>(TEntity entity)
        {
            DbPersister.Delete(entity);
        }

        public void Insert<TEntity>(TEntity entity)
        {
            DbPersister.Insert(entity);
        }

        bool IDbSession.PersistChanges<TEntity>(TEntity entity)
        {
            return DbEntityPersister.PersistChanges(entity);
        }
    }
}