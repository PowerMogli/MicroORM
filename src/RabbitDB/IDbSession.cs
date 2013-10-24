using System;
using System.Linq.Expressions;
using MicroORM.Query;

namespace MicroORM.Base
{
    internal interface IDbSession : ITransactionalSession, IDisposable
    {
        void ExecuteCommand(string sql, params object[] args);
        void ExecuteStoredProcedure(StoredProcedure procedureObject);
        void ExecuteStoredProcedure(string storedProcedureName, params object[] arguments);
        T ExecuteStoredProcedure<T>(StoredProcedure procedureObject);
        T ExecuteStoredProcedure<T>(string storedProcedureName, params object[] arguments);
        V GetColumnValue<T, V>(Expression<Func<T, V>> selector, Expression<Func<T, bool>> criteria);
        T GetObject<T>(Expression<Func<T, bool>> criteria);
        T GetObject<T>(object primaryKey, string additionalPredicate = null);
        T GetObject<T>(object[] primaryKey, string additionalPredicate = null);
        ObjectSet<T> GetObjectSet<T>();
        ObjectSet<T> GetObjectSet<T>(Expression<Func<T, bool>> condition);
        ObjectSet<T> GetObjectSet<T>(string sql, params object[] args);
        ObjectSet<T> GetObjectSet<T>(IQuery query);
        T GetScalarValue<T>(string sql, params object[] args);
        bool PersistChanges<TEntity>(TEntity entity, bool isToDelete = false) where TEntity : Entity.Entity;
        void Update<T>(Expression<Func<T, bool>> criteria, params object[] setArguments);
        //void Update<T>(T data);
        void Load<TEntity>(TEntity entity) where TEntity : Entity.Entity;
        void Delete<TEntity>(TEntity entity);
        void Insert<T>(T data);
        ObjectReader<T> GetObjectReader<T>(IQuery query);
    }
}
