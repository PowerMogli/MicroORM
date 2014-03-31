namespace RabbitDB.Storage
{
    internal class PostgresDbProvider : TransactionalDbProvider, ITransactionalDbProvider, IDbProvider
    {
        internal PostgresDbProvider(string connectionString)
            : base(connectionString) { }

        public override string ProviderName { get { return "Npgsql"; } }
    }
}