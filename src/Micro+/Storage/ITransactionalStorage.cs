using System.Data;

namespace MicroORM.Storage
{
    interface ITransactionalProvider : IDbProvider
    {
        IDbTransaction BeginTransaction(IsolationLevel isolationLevel);
        IDbTransaction BeginTransaction();
    }
}
