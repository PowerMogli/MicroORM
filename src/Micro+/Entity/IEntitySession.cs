using System;
using MicroORM.Base;

namespace MicroORM.Entity
{
    internal interface IEntitySession : ITransactionalSession, IDisposable
    {
        void Load<TEntity>(TEntity entity) where TEntity : Entity;
        void Update<TEntity>(TEntity entity) where TEntity : Entity;
        void Insert<TEntity>(TEntity entity) where TEntity : Entity;
        void Delete<TEntity>(TEntity entity) where TEntity : Entity;
    }
}
