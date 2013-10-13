using System;
using System.Data;

namespace MicroORM.Base
{
    internal interface IEntitySession : ITransactionalSession, IDisposable
    {
        void Load<TEntity>(TEntity entity) where TEntity : Entity;
        void Update<TEntity>(TEntity entity) where TEntity : Entity;
        void Insert<TEntity>(TEntity entity) where TEntity : Entity;
    }
}
