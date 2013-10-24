using System.Data;

namespace MicroORM.Base
{
    internal interface ITransactionalSession
    {
        IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null);
    }
}
