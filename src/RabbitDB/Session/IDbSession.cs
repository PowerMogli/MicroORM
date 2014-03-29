using System;
using System.Linq.Expressions;

namespace RabbitDB.Base
{
    internal interface IDbSession : ITransactionalSession, IDisposable
    {
        void ExecuteCommand(string sql, params object[] args);
        bool PersistChanges<TEntity>(TEntity entity) where TEntity : Entity.Entity;
        void Update<T>(Expression<Func<T, bool>> criteria, params object[] setArguments);
        //void Update<T>(T data);
        void Load<TEntity>(TEntity entity) where TEntity : Entity.Entity;
        void Delete<TEntity>(TEntity entity);
        void Insert<T>(T data);
    }
}