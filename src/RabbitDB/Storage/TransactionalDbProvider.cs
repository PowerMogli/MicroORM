using RabbitDB.Expressions;
using System.Data;

namespace RabbitDB.Storage
{
    internal abstract class TransactionalDbProvider : DbProvider, ITransactionalDbProvider
    {
        internal TransactionalDbProvider(string connectionString)
            : base(connectionString) { }

        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (base._dbTransaction != null) return base._dbTransaction;
            if (base._dbConnection != null) return base._dbTransaction = base._dbConnection.BeginTransaction(isolationLevel);

            base.CreateConnection();
            return base._dbTransaction = base._dbConnection.BeginTransaction(isolationLevel);
        }

        public IDbTransaction BeginTransaction()
        {
            if (base._dbTransaction != null) return base._dbTransaction;
            if (base._dbConnection != null) return base._dbTransaction = base._dbConnection.BeginTransaction();

            base.CreateConnection();
            return base._dbTransaction = base._dbConnection.BeginTransaction();
        }

        public abstract override string ProviderName { get; }
    }
}