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
        private IDbProvider _provider = null;
        private DbEngine _dbEngine;

        public DbSession(string connectionString, DbEngine dbEngine)
        {
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentNullException("connectionString", "The connectionString cannot be null or empty!");

            _dbEngine = dbEngine;
            _provider = DbProviderFactory.GetProvider(dbEngine, connectionString);
        }

        public DbSession(Type assemblyType)
        {
            string connectionString = ConnectionStringRegistrar.GetFor(assemblyType);
            _dbEngine = DbEngineRegistrar.GetFor(assemblyType);
            _provider = DbProviderFactory.GetProvider(_dbEngine, connectionString);
        }

        public DbSession(string connectionString)
            : this(connectionString, DbEngine.SqlServer) { }

        ~DbSession()
        {
            Dispose();
        }

        public void ExecuteCommand(string sql, params object[] arguments)
        {
            _provider.ExecuteCommand(new SqlQuery(sql, QueryParameterCollection.Create(arguments)));
        }

        public void ExecuteStoredProcedure(StoredProcedure procedureObject)
        {
            _provider.ExecuteCommand(new StoredProcedureQuery(procedureObject));
        }

        public TEntity ExecuteStoredProcedure<TEntity>(StoredProcedure procedureObject)
        {
            ObjectSet<TEntity> objectSet = ((IDbSession)this).GetObjectSet<TEntity>(new StoredProcedureQuery(procedureObject));
            return objectSet.SingleOrDefault();
        }

        public void ExecuteStoredProcedure(string storedProcedureName, params object[] arguments)
        {
            _provider.ExecuteCommand(new StoredProcedureQuery(storedProcedureName, QueryParameterCollection.Create(arguments)));
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
            DbSchemaAllocator<TEntity>.Allocate();
            return _provider.ExecuteScalar<TEntity>(new SqlQuery(sql, QueryParameterCollection.Create(arguments)));
        }

        public ObjectSet<TEntity> GetObjectSet<TEntity>(string sql, params object[] arguments)
        {
            return ((IDbSession)this).GetObjectSet<TEntity>(new SqlQuery<TEntity>(sql, QueryParameterCollection.Create<TEntity>(arguments)));
        }

        ObjectSet<TEntity> IDbSession.GetObjectSet<TEntity>(IQuery query)
        {
            DbSchemaAllocator<TEntity>.Allocate();
            ObjectSet<TEntity> objectSet = new ObjectSet<TEntity>();
            return objectSet.Load(this, query);
        }

        ObjectReader<TEntity> IDbSession.GetObjectReader<TEntity>(IQuery query)
        {
            return _provider.ExecuteReader<TEntity>(query);
        }

        public ObjectSet<TEntity> GetObjectSet<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            ObjectSet<TEntity> objectSet = ((IDbSession)this).GetObjectSet<TEntity>(new SimpleExpressionQuery<TEntity>(condition));
            return objectSet;
        }

        public ObjectSet<TEntity> GetObjectSet<TEntity>()
        {
            var tableInfo = TableInfo.GetTableInfo(typeof(TEntity));
            return ((IDbSession)this).GetObjectSet<TEntity>(new SqlQuery(string.Format("select * from {0}", (_provider.EscapeName(tableInfo.Name)))));
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> criteria, params object[] setArguments)
        {

        }

        public void Update<TEntity>(TEntity data)
        {

        }

        public LastInsertId Insert<TEntity>(TEntity data)
        {
            DbSchemaAllocator<TEntity>.Allocate();
            TableInfo tableInfo = TableInfo.GetTableInfo(typeof(TEntity));
            string insertStatement = tableInfo.CreateInsertStatement(_provider);
            QueryParameterCollection arguments = QueryParameterCollection.Create<TEntity>(Utils.Utils.GetEntityArguments(data, tableInfo));

            return new LastInsertId(_provider.ExecuteScalar<object>(new SqlQuery(insertStatement, arguments)));
        }

        public bool PersistChanges()
        {
            return true;
        }

        public IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            ITransactionalDbProvider transactionalProvider = _provider as ITransactionalDbProvider;
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
            _provider.Dispose();
            DbSchemaAllocator.SchemaReader.Dispose();
            DbSchemaAllocator.SchemaReader = null;
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        internal void Load<TEntity>(TEntity entity) where TEntity : Entity.Entity
        {
            DbSchemaAllocator<TEntity>.Allocate();
            ObjectReader<TEntity> objectReader = _provider.ExecuteReader<TEntity>(new EntityQuery<TEntity>(entity));
            if (objectReader.Load(entity) == false) throw new Exception("Loading data was not successfull!");
        }
    }
}
