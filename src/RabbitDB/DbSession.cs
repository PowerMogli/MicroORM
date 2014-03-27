using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using RabbitDB.Mapping;
using RabbitDB.Query;
using RabbitDB.Query.Generic;
using RabbitDB.Query.StoredProcedure;
using RabbitDB.Reader;
using RabbitDB.Schema;
using RabbitDB.Storage;
using RabbitDB.Entity;

namespace RabbitDB.Base
{
    public class DbSession : IDisposable, IDbSession
    {
        private IDbProvider _dbProvider = null;
        private DbEngine _dbEngine;

        public DbSession(string connectionString, DbEngine dbEngine)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString", "The connectionString cannot be null or empty!");

            _dbEngine = dbEngine;
            Initialize(connectionString, _dbEngine);
        }

        public DbSession(Type assemblyType)
        {
            string connectionString = Registrar<string>.GetFor(assemblyType);
            _dbEngine = Registrar<DbEngine>.GetFor(assemblyType);
            Initialize(connectionString, _dbEngine);
        }

        public DbSession(string connectionString)
            : this(connectionString, DbEngine.SqlServer) { }

        private void Initialize(string connectionString, DbEngine dbEngine)
        {
            _dbProvider = DbProviderFactory.GetProvider(_dbEngine, connectionString);
        }

        ~DbSession()
        {
            Dispose();
        }

        private IDbPersister _dbPersister;
        private IDbPersister DbPersister
        {
            get
            {
                if (_dbProvider == null)
                {
                    throw new InvalidOperationException("DbProvider is not initialized");
                }

                return _dbPersister ?? (_dbPersister = new DbPersister(_dbProvider));
            }
        }

        private DbEntityPersister _dbEntityPersister;
        private DbEntityPersister DbEntityPersister
        {
            get
            {
                if (_dbProvider == null)
                {
                    throw new InvalidOperationException("DbProvider is not initialized");
                }

                return _dbEntityPersister ?? (_dbEntityPersister = new DbEntityPersister(_dbProvider, DbPersister));
            }
        }

        public static Configuration Configuration { get { return Configuration.Instance; } }

        public void ExecuteCommand(string sql, params object[] arguments)
        {
            _dbProvider.ExecuteCommand(new SqlQuery(sql, QueryParameterCollection.Create(arguments)));
        }

        public void ExecuteStoredProcedure(StoredProcedure procedureObject)
        {
            _dbProvider.ExecuteCommand(new StoredProcedureQuery(procedureObject));
        }

        public TEntity ExecuteStoredProcedure<TEntity>(StoredProcedure procedureObject)
        {
            EntitySet<TEntity> objectSet = ((IDbSession)this).GetEntitySet<TEntity>(new StoredProcedureQuery(procedureObject));
            return objectSet.SingleOrDefault();
        }

        public void ExecuteStoredProcedure(string storedProcedureName, params object[] arguments)
        {
            _dbProvider.ExecuteCommand(new StoredProcedureQuery(storedProcedureName, QueryParameterCollection.Create(arguments)));
        }

        public TEntity ExecuteStoredProcedure<TEntity>(string storedProcedureName, params object[] arguments)
        {
            var query = new StoredProcedureQuery(storedProcedureName, QueryParameterCollection.Create<TEntity>(arguments));
            EntitySet<TEntity> objectSet = ((IDbSession)this).GetEntitySet<TEntity>(query);
            return objectSet.SingleOrDefault();
        }

        public TEntity GetEntity<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            EntitySet<TEntity> objectSet = ((IDbSession)this).GetEntitySet<TEntity>(new ExpressionQuery<TEntity>(condition));
            return objectSet.FirstOrDefault();
        }

        public TEntity GetEntity<TEntity>(object primaryKey, string additionalPredicate = null)
        {
            return this.GetEntity<TEntity>(new object[] { primaryKey }, additionalPredicate);
        }

        public TEntity GetEntity<TEntity>(object[] primaryKeys, string additionalPredicate = null)
        {
            if (primaryKeys == null || primaryKeys.Length == 0)
                throw new PrimaryKeyException("No primary Keys provided!");

            var entitySet = ((IDbSession)this).GetEntitySet<TEntity>(new SqlQuery<TEntity>(primaryKeys, additionalPredicate, null));
            return entitySet.SingleOrDefault();
        }

        public V GetColumnValue<TEntity, V>(Expression<Func<TEntity, V>> selector, Expression<Func<TEntity, bool>> criteria)
        {
            return default(V);
        }

        public TEntity GetScalarValue<TEntity>(string sql, params object[] arguments)
        {
            return _dbProvider.ExecuteScalar<TEntity>(new SqlQuery(sql, QueryParameterCollection.Create(arguments)));
        }

        EntitySet<TEntity> IDbSession.GetEntitySet<TEntity>(IQuery query)
        {
            EntitySet<TEntity> objectSet = new EntitySet<TEntity>();
            return objectSet.Load(this, query);
        }

        public EntitySet<TEntity> GetEntitySet<TEntity>(string sql, params object[] arguments)
        {
            return ((IDbSession)this).GetEntitySet<TEntity>(new SqlQuery<TEntity>(sql, QueryParameterCollection.Create<TEntity>(arguments)));
        }

        public EntitySet<TEntity> GetEntitySet<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            return ((IDbSession)this).GetEntitySet<TEntity>(new ExpressionQuery<TEntity>(condition));
        }

        public EntitySet<TEntity> GetEntitySet<TEntity>()
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            return ((IDbSession)this).GetEntitySet<TEntity>(new SqlQuery(tableInfo.GetBaseSelect(_dbProvider)));
        }

        EntityReader<TEntity> IDbSession.GetEntityReader<TEntity>(IQuery query)
        {
            return _dbProvider.ExecuteReader<TEntity>(query);
        }

        public EntityReader<TEntity> GetEntityReader<TEntity>(string sql, params object[] arguments)
        {
            return ((IDbSession)this).GetEntityReader<TEntity>(new SqlQuery<TEntity>(sql, QueryParameterCollection.Create<TEntity>(arguments)));
        }

        public EntityReader<TEntity> GetEntityReader<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            return ((IDbSession)this).GetEntityReader<TEntity>(new ExpressionQuery<TEntity>(condition));
        }

        public EntityReader<TEntity> GetEntityReader<TEntity>()
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            return ((IDbSession)this).GetEntityReader<TEntity>(new SqlQuery(tableInfo.GetBaseSelect(_dbProvider)));
        }

        public MultiEntityReader ExecuteMultiple(string sql, params object[] arguments)
        {
            IDataReader dataReader = _dbProvider.ExecuteReader(new SqlQuery(sql, QueryParameterCollection.Create(arguments)));
            return new MultiEntityReader(dataReader, _dbProvider);
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
            EntityReader<TEntity> objectReader = (EntityReader<TEntity>)_dbProvider.ExecuteReader<TEntity>(new EntityQuery<TEntity>(entity));
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

        public IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            ITransactionalDbProvider transactionalProvider = _dbProvider as ITransactionalDbProvider;
            if (transactionalProvider == null)
                throw new NotSupportedException(
                    string.Format(@"This type of DbEngine ('{0}') does not implement the interface ITransactionalDbProvider
                                  and so does not support transactional behavior.", _dbEngine));

            return isolationLevel.HasValue
                ? transactionalProvider.BeginTransaction(isolationLevel.Value)
                : transactionalProvider.BeginTransaction();
        }

        private void Dispose()
        {
            if (_dbPersister != null)
            {
                _dbPersister = null;
            }

            _dbProvider.Dispose();
            DbSchemaAllocator.SchemaReader.Dispose();
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
        }
    }
}