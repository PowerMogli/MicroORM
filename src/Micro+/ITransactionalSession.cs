using System.Data;

namespace MicroORM.Base
{
    public interface ITransactionalSession
    {
        IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null);
    }
}
