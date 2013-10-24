using System.Data;
using MicroORM.Schema;

namespace MicroORM.Storage
{
    internal class SqlDbProvider : DbProvider, ITransactionalDbProvider, IDbProvider
    {
        private const string _providerName = "System.Data.SqlClient";

        public override string ParameterPrefix { get { return "@"; } }
        public override string ProviderName { get { return _providerName; } }

        public override string ScopeIdentity
        {
            get { return "; Select SCOPE_IDENTITY() as id"; }
        }

        internal SqlDbProvider(string connectionString)
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

        public override string EscapeName(string value)
        {
            return "[" + value + "]";
        }
    }
}
