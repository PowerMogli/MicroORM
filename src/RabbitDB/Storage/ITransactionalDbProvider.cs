using System.Data;

namespace MicroORM.Storage
{
    interface ITransactionalDbProvider : IDbProvider
    {
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);
        IDbTransaction BeginTransaction();
    }
}
