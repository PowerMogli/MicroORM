using RabbitDB.Schema;
namespace RabbitDB.Storage
{
    internal class DbProviderFactory
    {
        public static IDbProvider GetProvider(DbEngine engine, string connectionString)
        {
            switch (engine)
            {
                case DbEngine.SqlServer:
                    if (DbSchemaAllocator.SchemaReader == null)
                        DbSchemaAllocator.SchemaReader = new SqlDbSchemaReader(connectionString);
                    return new SqlDbProvider(connectionString);
                //case DbEngine.SqlServerCE:
                //    return new SqlServerCEProvider();
                //case DbEngine.MySql:
                //    return new MySqlProvider();
                //case DbEngine.PostgreSQL:
                //    return new PostgresProvider();
                ////case DbEngine.Oracle:
                ////    return new OracleProvider();
                //case DbEngine.SQLite:
                //    return new SqliteProvider();
            }
            throw new NotSupportedProviderException(string.Format("Unkown engine {0}!", engine.ToString()));
        }
    }
}
