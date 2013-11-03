using System;
using System.Linq;
using RabbitDB.Expressions;
using RabbitDB.Mapping;

namespace RabbitDB.Storage
{
    internal class SqlDbProvider : TransactionalDbProvider, ITransactionalDbProvider, IDbProvider
    {
        private const string _providerName = "System.Data.SqlClient";

        internal SqlDbProvider(string connectionString)
            : base(connectionString) { }

        public override string ProviderName { get { return _providerName; } }

        private IDbProviderExpressionBuildHelper _builderHelper;
        public override IDbProviderExpressionBuildHelper BuilderHelper
        {
            get { return _builderHelper ?? (_builderHelper = new ExpressionBuildHelper(this)); }
        }

        public override string ScopeIdentity
        {
            get { return "; SELECT CAST(SCOPE_IDENTITY() AS {0}) AS ID"; }
        }

        public override string ResolveScopeIdentity(TableInfo tableInfo)
        {
            Tuple<bool, string> result = tableInfo.GetIdentityType();
            return result.Item1 ? string.Format(this.ScopeIdentity, result.Item2) : string.Empty;
        }

        public override string EscapeName(string value)
        {
            if (value.Contains("[") && value.Contains("]"))
                return value;
            if (value.Contains("\""))
                return value;

            if (!value.Contains("."))
                return "[" + value + "]";

            return string.Join(".", value.Split('.').Select(d => "[" + d + "]"));
        }
    }
}