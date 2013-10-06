using System;
using System.Linq.Expressions;
using MicroORM.Base.Mapping;
using MicroORM.Base.Query;
using MicroORM.Base.Query.Generic;
using MicroORM.Base.Storage;

namespace MicroORM.Base
{
    public class Session : IDisposable
    {
        private string _connectionString = string.Empty;
        private IDbProvider _provider = null;

        public Session(string connectionString, DbEngine dbEngine)
        {
            _connectionString = connectionString;
            _provider = ProviderFactory.GetProvider(dbEngine, connectionString);
        }

        public Session(string connectionString)
            : this(connectionString, DbEngine.SqlServer) { }

        public void ExecuteCommand(string sql, params object[] args)
        {
            _provider.ExecuteCommand(new SqlQuery(sql, args));
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

        public V GetColumn<T, V>(Expression<Func<T, V>> selector, Expression<Func<T, bool>> criteria)
        {
            return default(V);
        }

        public T GetValue<T>(string sql, params object[] args)
        {
            return default(T);
        }

        public ObjectSet<T> GetObjectSet<T>(string sql, params object[] args)
        {
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
