namespace RabbitDB.Storage
{
    internal class SqlDbProvider : TransactionalDbProvider, ITransactionalDbProvider, IDbProvider
    {
        internal SqlDbProvider(string connectionString)
            : base(connectionString) { }

        public override string ProviderName { get { return "System.Data.SqlClient"; } }
    }
}