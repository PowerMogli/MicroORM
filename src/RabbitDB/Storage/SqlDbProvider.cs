using System.Data;
using RabbitDB.Schema;
using RabbitDB.Expressions;

namespace RabbitDB.Storage
{
    internal class SqlDbProvider : DbProvider, ITransactionalDbProvider, IDbProvider
    {
        private const string _providerName = "System.Data.SqlClient";

        public override string ParameterPrefix { get { return "@"; } }
        public override string ProviderName { get { return _providerName; } }

        private IDbProviderExpressionBuildHelper _builderHelper;
        public override IDbProviderExpressionBuildHelper BuilderHelper
        {
            get { return _builderHelper ?? (_builderHelper = new ExpressionBuildHelper(this)); }
        }

        public override string ScopeIdentity
        {
            get { return "; Select CAST(SCOPE_IDENTITY() AS {0}) as id"; }
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
