// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDialectFactory.cs" company="">
//   
// </copyright>
// <summary>
//   The sql dialect factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    using System;

    using RabbitDB.Schema;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The sql dialect factory.
    /// </summary>
    internal static class SqlDialectFactory
    {
        #region Methods

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="dbEngine">
        /// The db engine.
        /// </param>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <returns>
        /// The <see cref="SqlDialect"/>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// </exception>
        internal static SqlDialect Create(DbEngine dbEngine, string connectionString)
        {
            SqlDialect sqlDialect = null;

            switch (dbEngine)
            {
                case DbEngine.SqlServer:
                    sqlDialect = new MsSqlDialect(new SqlDbProvider(connectionString));
                    DbSchemaAllocator.SchemaReader = new MySqlDbSchemaReader(sqlDialect);
                    break;
                case DbEngine.SqlServerCe:
                    break;
                case DbEngine.MySql:
                    break;
                case DbEngine.PostgreSql:
                    sqlDialect = new PostgreSqlDialect(new PostgresDbProvider(connectionString));
                    DbSchemaAllocator.SchemaReader = new PostgreSqlDbSchemaReader(sqlDialect);
                    break;
                case DbEngine.Oracle:
                    break;
                case DbEngine.SqLite:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("dbEngine");
            }

            return sqlDialect;
        }

        #endregion
    }
}