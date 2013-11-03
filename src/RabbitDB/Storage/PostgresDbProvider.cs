using System.Linq;
using RabbitDB.Expressions;
using RabbitDB.Mapping;

namespace RabbitDB.Storage
{
    internal class PostgresDbProvider : TransactionalDbProvider, ITransactionalDbProvider, IDbProvider
    {
        private const string _providerName = "Npgsql";

        internal PostgresDbProvider(string connectionString)
            : base(connectionString) { }

        public override string ProviderName { get { return _providerName; } }

        public override string ScopeIdentity
        {
            get { return " returning {0}"; }
        }

        private IDbProviderExpressionBuildHelper _builderHelper;
        public override IDbProviderExpressionBuildHelper BuilderHelper
        {
            get { return _builderHelper ?? (_builderHelper = new PostgresExpressionBuilderHelper(this)); }
        }

        public override string ResolveScopeIdentity(TableInfo tableInfo)
        {
            IPropertyInfo propertyInfo = tableInfo.Columns.Where(column => column.ColumnAttribute.AutoNumber).FirstOrDefault();
            return string.Format(this.ScopeIdentity, base.EscapeName(propertyInfo.ColumnAttribute.ColumnName));
        }
    }
}