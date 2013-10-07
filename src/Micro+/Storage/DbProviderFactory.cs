using System;

namespace MicroORM.Storage
{
    internal class DbProviderFactory
    {
        public static IDbProvider GetProvider(DbEngine engine, string connectionString)
        {
            switch (engine)
            {
                case DbEngine.SqlServer:
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
            throw new Exception("Unkown provider");
        }
    }
}
