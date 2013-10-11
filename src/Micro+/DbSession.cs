using System;
using System.Data;
using System.Linq.Expressions;
using MicroORM.Mapping;
using MicroORM.Query;
using MicroORM.Query.Generic;
using MicroORM.Storage;

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

        public T ExecuteStoredProcedure<T>(ProcedureObject procedureObject)
        {
            ObjectSet<T> objectSet = ((IDbSession)this).GetObjectSet<T>(new StoredProcedureQuery(procedureObject));
            return objectSet.SingleOrDefault();
        }

        public T GetObject<T>(Expression<Func<T, bool>> condition)
        {
            ObjectSet<T> objectSet = ((IDbSession)this).GetObjectSet<T>(new SimpleExpressionQuery<T>(condition));
            return objectSet.SingleOrDefault();
        }

        public T GetObject<T>(object primaryKey, string additionalPredicate = null, params object[] arguments)
        {
            ObjectSet<T> objectSet = ((IDbSession)this).GetObjectSet<T>(new SqlQuery<T>(primaryKey, additionalPredicate, arguments));
            return objectSet.SingleOrDefault();
        }

        public V GetColumnValue<T, V>(Expression<Func<T, V>> selector, Expression<Func<T, bool>> criteria)
        {
            return default(V);
        }

        public T GetValue<T>(string sql, params object[] arguments)
        {
            return _provider.ExecuteScalar<T>(new SqlQuery(sql, arguments));
        }

        public ObjectSet<T> GetObjectSet<T>(string sql, params object[] arguments)
        {
            return ((IDbSession)this).GetObjectSet<T>(new SqlQuery<T>(sql, arguments));
        }

        ObjectSet<T> IDbSession.GetObjectSet<T>(IQuery query)
        {
            ObjectSet<T> objectSet = new ObjectSet<T>();
            return objectSet.Load(this, query);
        }

        ObjectReader<T> IDbSession.GetObjectReader<T>(IQuery query)
        {
            return _provider.ExecuteReader<T>(query);
        }

        public ObjectSet<T> GetObjectSet<T>(Expression<Func<T, bool>> condition)
        {
            ObjectSet<T> objectSet = ((IDbSession)this).GetObjectSet<T>(new SimpleExpressionQuery<T>(condition));
            return objectSet;
        }

        public ObjectSet<T> GetObjectSet<T>()
        {
            var typeMapping = TableInfo.GetTableInfo(typeof(T));
            return ((IDbSession)this).GetObjectSet<T>(new SqlQuery(string.Format("select * from {0}", (_provider.EscapeName(typeMapping.PersistentAttribute.EntityName)))));
        }

        public void Update<T>(Expression<Func<T, bool>> criteria, params object[] setArguments)
        {

        }

        public void Update<T>(T data)
        {

        }
        
        public TEntity Load<TEntity>(TEntity entity)
        {
            TableInfo tableInfo = TableInfo.GetTableInfo(entity.GetType());
            object primaryKey = tableInfo.GetPrimaryKey(entity);

            return GetObject<TEntity>(primaryKey);
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
    }
}
