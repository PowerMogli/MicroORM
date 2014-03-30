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
                    var selectBuilder = new SelectSqlBuilder(SqlDbProviderAccessor.DbProvider, tableInfo);
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
                    var deleteBuilder = new DeleteSqlBuilder(SqlDbProviderAccessor.DbProvider, tableInfo);
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
                    var insertBuilder = new InsertSqlBuilder(SqlDbProviderAccessor.DbProvider, tableInfo);
                    InternalInsertStatement = insertBuilder.CreateStatement();
                }
                return InternalInsertStatement;
            }
        }

        internal static string GetUpdateStatement(KeyValuePair<string, object>[] arguments)
        {
            var tableInfo = TableInfo<TEntity>.GetTableInfo;
            var insertBuilder = new UpdateSqlBuilder(SqlDbProviderAccessor.DbProvider, tableInfo, arguments);
            return insertBuilder.CreateStatement();
        }
    }
}