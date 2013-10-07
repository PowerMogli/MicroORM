using System;
using System.Data;
using System.Linq.Expressions;
using MicroORM.Mapping;
using MicroORM.Query;
using MicroORM.Query.Generic;
using MicroORM.Storage;

namespace MicroORM.Base
{
    public class Session : IDisposable, IDbSession
    {
        private IDbProvider _provider = null;
        private DbEngine _dbEngine;

        public Session(string connectionString, DbEngine dbEngine)
        {
            _dbEngine = dbEngine;
            _provider = ProviderFactory.GetProvider(dbEngine, connectionString);
        }

        public Session(string connectionString)
            : this(connectionString, DbEngine.SqlServer) { }

        ~Session()
        {
            Dispose();
        }

        public IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            ITransactionalProvider transactionalProvider = _provider as ITransactionalProvider;
            if (transactionalProvider == null)
                throw new NotSupportedException(string.Format("This type of DbEngine ('{0}') does not support transactional behavior", _dbEngine));

            return isolationLevel.HasValue
                ? transactionalProvider.BeginTransaction(isolationLevel.Value)
                : transactionalProvider.BeginTransaction();
        }

        public void ExecuteCommand(string sql, params object[] args)
        {
            _provider.ExecuteCommand(new SqlQuery(sql, args));
        }

        public void ExecuteStoredProcedure(ProcedureObject procedureObject)
        {
            _provider.ExecuteCommand(new StoredProcedureQuery(procedureObject));
        }

        public void ExecuteStoredProcedure<T>(ProcedureObject procedureObject)
        {

        }

        public T GetObject<T>(Expression<Func<T, bool>> criteria)
        {
            return default(T);
        }

        public T GetObject<T>(object primaryKey, string additionalPredicate = null, params object[] args)
        {
            using (ObjectReader<T> objectReader = _provider.ExecuteReader<T>(new SqlQuery<T>(primaryKey, additionalPredicate, args)))
            {
                if (objectReader.Read() == false) return default(T);

                return objectReader.Current;
            }
        }

        public V GetColumnValue<T, V>(Expression<Func<T, V>> selector, Expression<Func<T, bool>> criteria)
        {
            return default(V);
        }

        public T GetValue<T>(string sql, params object[] args)
        {
            return default(T);
        }

        public ObjectSet<T> GetObjectSet<T>(string sql, params object[] args)
        {
            using (ObjectReader<T> ObjectReader = _provider.ExecuteReader<T>(new SqlQuery<T>(sql, args)))
            {

            }

            return default(ObjectSet<T>);
        }

        public ObjectSet<T> GetObjectSet<T>(Expression<Func<T, bool>> condition)
        {
            return default(ObjectSet<T>);
        }

        public ObjectSet<T> GetObjectSet<T>()
        {
            var typeMapping = TypeMapping.GetTypeMapping(typeof(T));
            var sql = string.Format("select * from {0}", (_provider.EscapeName(typeMapping.PersistentAttribute.EntityName)));
            return GetObjectSet<T>(sql);
        }

        public void Update<T>(Expression<Func<T, bool>> criteria, params object[] setArguments)
        {

        }

        public void Update<T>(T data)
        {

        }

        public bool PersistChanges()
        {
            return true;
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
