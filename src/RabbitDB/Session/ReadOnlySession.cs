using RabbitDB.Mapping;
using RabbitDB.Query;
using RabbitDB.Query.Generic;
using RabbitDB.Reader;
using RabbitDB.Storage;
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace RabbitDB.Base
{
    public class ReadOnlySession : BaseDbSession, IReadOnlySession
    {
        public ReadOnlySession(string connectionString, DbEngine dbEngine)
            : base(connectionString, dbEngine) { }

        public ReadOnlySession(Type assemblyType)
            : base(assemblyType) { }

        public ReadOnlySession(string connectionString)
            : this(connectionString, DbEngine.SqlServer) { }

        public V GetColumnValue<TEntity, V>(Expression<Func<TEntity, V>> selector, Expression<Func<TEntity, bool>> criteria)
        {
            return default(V);
        }

        public TEntity GetEntity<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            EntitySet<TEntity> objectSet = ((IBaseDbSession)this).GetEntitySet<TEntity>(new ExpressionQuery<TEntity>(condition));
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

            var entitySet = ((IBaseDbSession)this).GetEntitySet<TEntity>(new SqlQuery<TEntity>(primaryKeys, additionalPredicate, null));
            return entitySet.SingleOrDefault();
        }

        public TEntity GetScalarValue<TEntity>(string sql, params object[] arguments)
        {
            return _dbProvider.ExecuteScalar<TEntity>(new SqlQuery(sql, QueryParameterCollection.Create(arguments)));
        }

        public EntitySet<TEntity> GetEntitySet<TEntity>(string sql, params object[] arguments)
        {
            return ((IBaseDbSession)this).GetEntitySet<TEntity>(new SqlQuery<TEntity>(sql, QueryParameterCollection.Create<TEntity>(arguments)));
        }

        public EntitySet<TEntity> GetEntitySet<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            return ((IBaseDbSession)this).GetEntitySet<TEntity>(new ExpressionQuery<TEntity>(condition));
        }

        public EntitySet<TEntity> GetEntitySet<TEntity>()
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            return ((IBaseDbSession)this).GetEntitySet<TEntity>(new SqlQuery(tableInfo.GetBaseSelect(_dbProvider)));
        }

        public EntityReader<TEntity> GetEntityReader<TEntity>(string sql, params object[] arguments)
        {
            return ((IBaseDbSession)this).GetEntityReader<TEntity>(new SqlQuery<TEntity>(sql, QueryParameterCollection.Create<TEntity>(arguments)));
        }

        public EntityReader<TEntity> GetEntityReader<TEntity>(Expression<Func<TEntity, bool>> condition)
        {
            return ((IBaseDbSession)this).GetEntityReader<TEntity>(new ExpressionQuery<TEntity>(condition));
        }

        public EntityReader<TEntity> GetEntityReader<TEntity>()
        {
            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;
            return ((IBaseDbSession)this).GetEntityReader<TEntity>(new SqlQuery(tableInfo.GetBaseSelect(_dbProvider)));
        }

        public MultiEntityReader ExecuteMultiple(string sql, params object[] arguments)
        {
            IDataReader dataReader = _dbProvider.ExecuteReader(new SqlQuery(sql, QueryParameterCollection.Create(arguments)));
            return new MultiEntityReader(dataReader, _dbProvider);
        }
    }
}