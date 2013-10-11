using System;
using System.Data;
using System.Linq.Expressions;
using MicroORM.Mapping;
using MicroORM.Query;
using MicroORM.Query.Generic;
using MicroORM.Storage;
using System.Linq;

namespace MicroORM.Base
{
    public class DbSession : IDisposable, IDbSession
    {
        private IDbProvider _provider = null;
        private DbEngine _dbEngine;

        public DbSession(string connectionString, DbEngine dbEngine)
        {
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
            _provider.ExecuteCommand(new SqlQuery(sql, arguments));
        }

        public void ExecuteStoredProcedure(ProcedureObject procedureObject)
        {
            _provider.ExecuteCommand(new StoredProcedureQuery(procedureObject));
        }

        public void ExecuteStoredProcedure(string storedProcedureName, params object[] arguments)
        {
            _provider.ExecuteCommand(new StoredProcedureQuery(storedProcedureName, arguments));
        }

        public TEntity ExecuteStoredProcedure<TEntity>(ProcedureObject procedureObject)
        {
            ObjectSet<TEntity> objectSet = ((IDbSession)this).GetObjectSet<TEntity>(new StoredProcedureQuery(procedureObject));
            return objectSet.SingleOrDefault();
        }

        public TEntity GetObject<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            ObjectSet<TEntity> objectSet = ((IDbSession)this).GetObjectSet<TEntity>(new SimpleExpressionQuery<TEntity>(condition));
            return objectSet.SingleOrDefault();
        }

        public TEntity GetObject<TEntity>(object primaryKey, string additionalPredicate = null, params object[] arguments)
        {
            ObjectSet<TEntity> objectSet = ((IDbSession)this).GetObjectSet<TEntity>(new SqlQuery<TEntity>(primaryKey, additionalPredicate, arguments));
            return objectSet.SingleOrDefault();
        }

        public V GetColumnValue<TEntity, V>(Expression<Func<TEntity, V>> selector, Expression<Func<TEntity, bool>> criteria)
        {
            return default(V);
        }

        public TEntity GetValue<TEntity>(string sql, params object[] arguments)
        {
            return _provider.ExecuteScalar<TEntity>(new SqlQuery(sql, arguments));
        }

        public ObjectSet<TEntity> GetObjectSet<TEntity>(string sql, params object[] arguments)
        {
            return ((IDbSession)this).GetObjectSet<TEntity>(new SqlQuery<TEntity>(sql, arguments));
        }

        ObjectSet<TEntity> IDbSession.GetObjectSet<TEntity>(IQuery query)
        {
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
            var typeMapping = TableInfo.GetTableInfo(typeof(TEntity));
            return ((IDbSession)this).GetObjectSet<TEntity>(new SqlQuery(string.Format("select * from {0}", (_provider.EscapeName(typeMapping.PersistentAttribute.EntityName)))));
        }

        public void Update<TEntity>(Expression<Func<TEntity, bool>> criteria, params object[] setArguments)
        {

        }

        public void Update<TEntity>(TEntity data)
        {

        }

        public void Insert<TEntity>(TEntity data)
        {

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
        }

        void IDisposable.Dispose()
        {
            this.Dispose();
        }

        public void Load<TEntity>(TEntity entity) where TEntity : Entity
        {
            ObjectReader<TEntity> objectReader = _provider.ExecuteReader<TEntity>(new EntityQuery<TEntity>(entity));
            if (objectReader.Load(entity) == false) throw new Exception("Das Laden war nicht erfolgreich");
        }
    }
}
