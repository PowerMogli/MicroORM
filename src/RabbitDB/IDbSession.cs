using System;
using System.Linq.Expressions;
using RabbitDB.Query;
using RabbitDB.Query.StoredProcedure;
using RabbitDB.Reader;

namespace RabbitDB.Base
{
    internal interface IDbSession : ITransactionalSession, IDisposable
    {
        void ExecuteCommand(string sql, params object[] args);
        void ExecuteStoredProcedure(StoredProcedure procedureObject);
        void ExecuteStoredProcedure(string storedProcedureName, params object[] arguments);
        T ExecuteStoredProcedure<T>(StoredProcedure procedureObject);
        T ExecuteStoredProcedure<T>(string storedProcedureName, params object[] arguments);
        V GetColumnValue<T, V>(Expression<Func<T, V>> selector, Expression<Func<T, bool>> criteria);
        T GetEntity<T>(Expression<Func<T, bool>> criteria);
        T GetEntity<T>(object primaryKey, string additionalPredicate = null);
        T GetEntity<T>(object[] primaryKey, string additionalPredicate = null);
        EntitySet<T> GetEntitySet<T>();
        EntitySet<T> GetEntitySet<T>(Expression<Func<T, bool>> condition);
        EntitySet<T> GetEntitySet<T>(string sql, params object[] args);
        EntitySet<T> GetEntitySet<T>(IQuery query);
        MultiEntityReader ExecuteMultiple(string sql, params object[] arguments);
        T GetScalarValue<T>(string sql, params object[] args);
        bool PersistChanges<TEntity>(TEntity entity) where TEntity : Entity.Entity;
        void Update<T>(Expression<Func<T, bool>> criteria, params object[] setArguments);
        //void Update<T>(T data);
        void Load<TEntity>(TEntity entity) where TEntity : Entity.Entity;
        void Delete<TEntity>(TEntity entity);
        void Insert<T>(T data);
        EntityReader<T> GetEntityReader<T>(IQuery query);
        EntityReader<T> GetEntityReader<T>();
        EntityReader<T> GetEntityReader<T>(Expression<Func<T, bool>> condition);
        EntityReader<T> GetEntityReader<T>(string sql, params object[] args);
    }
}
