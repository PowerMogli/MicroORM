using System.Data;

namespace RabbitDB.Storage
{
    interface ITransactionalDbProvider : IDbProvider
    {
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);
        IDbTransaction BeginTransaction();
    }
}
