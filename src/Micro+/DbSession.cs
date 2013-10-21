using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using MicroORM.Mapping;
using MicroORM.Query;
using MicroORM.Query.Generic;
using MicroORM.Schema;
using MicroORM.Storage;

namespace MicroORM.Base
{
    public class DbSession : IDisposable, IDbSession
    {
        private IDbProvider _dbProvider = null;
        private DbEngine _dbEngine;
        private DbEntityPersister _dbEntityPersister;

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
            _dbEntityPersister = new DbEntityPersister(_dbProvider);
        }

        ~DbSession()
        {
            Dispose();
        }

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
            ObjectSet<TEntity> objectSet = ((IDbSession)this).GetObjectSet<TEntity>(new StoredProcedureQuery(procedureObject));
            return objectSet.SingleOrDefault();
        }

        public void ExecuteStoredProcedure(string storedProcedureName, params object[] arguments)
        {
            _dbProvider.ExecuteCommand(new StoredProcedureQuery(storedProcedureName, QueryParameterCollection.Create(arguments)));
        }

        public TEntity ExecuteStoredProcedure<TEntity>(string storedProcedureName, params object[] arguments)
        {
            ObjectSet<TEntity> objectSet = ((IDbSession)this).GetObjectSet<TEntity>(new StoredProcedureQuery(storedProcedureName, QueryParameterCollection.Create<TEntity>(arguments)));
            return objectSet.SingleOrDefault();
        }

        public TEntity GetObject<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            ObjectSet<TEntity> objectSet = ((IDbSession)this).GetObjectSet<TEntity>(new SimpleExpressionQuery<TEntity>(condition));
            return objectSet.SingleOrDefault();
        }

        public TEntity GetObject<TEntity>(object primaryKey, string additionalPredicate = null)
        {
            return this.GetObject<TEntity>(new object[] { primaryKey }, additionalPredicate);
        }

        public TEntity GetObject<TEntity>(object[] primaryKeys, string additionalPredicate = null)
        {
            if (primaryKeys == null || primaryKeys.Length == 0)
                throw new PrimaryKeyException("No primary Keys provided!");

            ObjectSet<TEntity> objectSet = ((IDbSession)this).GetObjectSet<TEntity>(new SqlQuery<TEntity>(primaryKeys, additionalPredicate, null));
            return objectSet.SingleOrDefault();
        }

        public V GetColumnValue<TEntity, V>(Expression<Func<TEntity, V>> selector, Expression<Func<TEntity, bool>> criteria)
        {
            return default(V);
        }

        public TEntity GetScalarValue<TEntity>(string sql, params object[] arguments)
        {
            return _dbProvider.ExecuteScalar<TEntity>(new SqlQuery(sql, QueryParameterCollection.Create(arguments)));
        }

        public ObjectSet<TEntity> GetObjectSet<TEntity>(string sql, params object[] arguments)
        {
            return ((IDbSession)this).GetObjectSet<TEntity>(new SqlQuery<TEntity>(sql, QueryParameterCollection.Create<TEntity>(arguments)));
        }

        ObjectSet<TEntity> IDbSession.GetObjectSet<TEntity>(IQuery query)
        {
            ObjectSet<TEntity> objectSet = new ObjectSet<TEntity>();
            return objectSet.Load(this, query);
        }

        ObjectReader<TEntity> IDbSession.GetObjectReader<TEntity>(IQuery query)
        {
            return _dbProvider.ExecuteReader<TEntity>(query);
        }

        public ObjectSet<TEntity> GetObjectSet<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            return ((IDbSession)this).GetObjectSet<TEntity>(new SimpleExpressionQuery<TEntity>(condition));
        }

        public ObjectSet<TEntity> GetObjectSet<TEntity>()
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            return ((IDbSession)this).GetObjectSet<TEntity>(new SqlQuery(string.Format("SELECT * FROM {0}", _dbProvider.EscapeName(tableInfo.Name))));
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> criteria, params object[] setArguments)
        {

        }

        public void Update<TEntity>(TEntity entity)
        {
            Tuple<bool, string, QueryParameterCollection> result = _dbEntityPersister.PrepareForUpdate(entity);
            _dbEntityPersister.Update<TEntity>(new SqlQuery(result.Item2, result.Item3));
        }

        void IDbSession.Load<TEntity>(TEntity entity)
        {
            ObjectReader<TEntity> objectReader = _dbProvider.ExecuteReader<TEntity>(new EntityQuery<TEntity>(entity));
            if (objectReader.Load(entity) == false) throw new Exception("Loading data was not successfull!");
        }

        public void Delete<TEntity>(TEntity entity)
        {
            _dbEntityPersister.Delete(entity);
        }

        public void Insert<TEntity>(TEntity entity)
        {
            _dbEntityPersister.Insert(entity);
        }

        bool IDbSession.PersistChanges<TEntity>(TEntity entity)
        {
            try
            {
                _dbEntityPersister.PersistChanges(entity);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
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
            _dbProvider.Dispose();
            DbSchemaAllocator.SchemaReader.Dispose();
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
        }
    }
}
