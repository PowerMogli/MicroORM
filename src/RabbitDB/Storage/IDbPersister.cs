using RabbitDB.Query;

namespace RabbitDB.Storage
{
    internal interface IDbPersister
    { 
        void Update<TEntity>(IQuery query);
        void Delete<TEntity>(TEntity entity);
        void Insert<TEntity>(TEntity entity);
    }
}