using RabbitDB.Mapping;
using RabbitDB.Session;
using System.Collections.Generic;

namespace RabbitDB.SqlBuilder
{
    internal static class SqlBuilder<TEntity>
    {
        private static string InternalSelectStatement { get; set; }
        private static string InternalInsertStatement { get; set; }
        private static string InternalDeleteStatement { get; set; }

        internal static string SelectStatement
        {
            get
            {
                if (string.IsNullOrWhiteSpace(InternalSelectStatement))
                {
                    var tableInfo = TableInfo<TEntity>.GetTableInfo;
                    var selectBuilder = new SelectSqlBuilder(DbProviderAccessor.SqlDialect, tableInfo);
                    InternalSelectStatement = selectBuilder.CreateStatement();
                }
                return InternalSelectStatement;
            }
        }

        internal static string DeleteStatement
        {
            get
            {
                if (string.IsNullOrWhiteSpace(InternalDeleteStatement))
                {
                    var tableInfo = TableInfo<TEntity>.GetTableInfo;
                    var deleteBuilder = new DeleteSqlBuilder(DbProviderAccessor.SqlDialect, tableInfo);
                    InternalDeleteStatement = deleteBuilder.CreateStatement();
                }
                return InternalDeleteStatement;
            }
        }

        internal static string InsertStatement
        {
            get
            {
                if (string.IsNullOrWhiteSpace(InternalInsertStatement))
                {
                    var tableInfo = TableInfo<TEntity>.GetTableInfo;
                    var insertBuilder = new InsertSqlBuilder(DbProviderAccessor.SqlDialect, tableInfo);
                    InternalInsertStatement = insertBuilder.CreateStatement();
                }
                return InternalInsertStatement;
            }
        }

        internal static string GetUpdateStatement(KeyValuePair<string, object>[] arguments)
        {
            var tableInfo = TableInfo<TEntity>.GetTableInfo;
            var updateBuilder = new UpdateSqlBuilder(DbProviderAccessor.SqlDialect, tableInfo, arguments);
            return updateBuilder.CreateStatement();
        }
    }
}