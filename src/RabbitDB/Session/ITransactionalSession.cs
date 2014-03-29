using System.Data;

namespace RabbitDB.Base
{
    internal interface ITransactionalSession
    {
        IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null);
    }
}
