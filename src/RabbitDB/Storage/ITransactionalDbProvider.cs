using System.Data;

namespace RabbitDB.Storage
{
    interface ITransactionalDbProvider
    {
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);
        IDbTransaction BeginTransaction();
    }
}
