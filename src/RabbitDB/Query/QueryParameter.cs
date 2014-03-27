using RabbitDB.Mapping;
using RabbitDB.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace RabbitDB.Query
{
    class QueryParameter
    {
        internal DbType DbType { get; private set; }
        internal string Name { get; private set; }
        internal int? Size { get; private set; }
        internal object Value { get; private set; }

        internal QueryParameter(string name, DbType dbType, object value, int size = -1)
        {
            this.Name = name;
            this.DbType = dbType;
            this.Value = value;
            this.Size = size > 0 ? new Nullable<int>(size) : null;
        }

        internal static QueryParameter CreateFromDbParameter(IDbDataParameter dbDataParameter)
        {
            return new QueryParameter(dbDataParameter.ParameterName, dbDataParameter.DbType, dbDataParameter.Value, dbDataParameter.Size);
        }

        internal static QueryParameter CreateFromRegular(int index, object[] arguments)
        {
            Type argumentType = arguments[index].GetType();
            if (argumentType.IsEnum) argumentType = typeof(Int32);
            return new QueryParameter(index.ToString(CultureInfo.InvariantCulture), TypeConverter.ToDbType(argumentType), arguments[index]);
        }

        internal static QueryParameter CreateFromKeyValuePairs(KeyValuePair<string, object> keyValuePair, TableInfo tableInfo)
        {
            Type argumentType = null;
            if (keyValuePair.Value != null)
            {
                argumentType = keyValuePair.Value.GetType();
                if (argumentType.IsEnum) argumentType = typeof(Int32);
            }

            var isColumn = tableInfo != null && tableInfo.IsColumn(keyValuePair.Key);
            return new QueryParameter(
                    keyValuePair.Key,
                    isColumn ? tableInfo.ConvertToDbType(keyValuePair.Key) : TypeConverter.ToDbType(argumentType),
                    EvaluateParameterValue(tableInfo, keyValuePair),
                    isColumn ? tableInfo.GetColumnSize(keyValuePair.Key) : -1);
        }

        private static object EvaluateParameterValue(TableInfo tableInfo, KeyValuePair<string, object> kvp)
        {
            if (tableInfo == null)
                return kvp.Value ?? DBNull.Value;

            if (kvp.Value != null) return kvp.Value;
            DbColumn dbColumn = tableInfo.DbTable.DbColumns.FirstOrDefault(column => column.Name == kvp.Key && column.IsNullable);
            if (dbColumn == null) return DBNull.Value;

            if (string.IsNullOrWhiteSpace(dbColumn.DefaultValue))
                return DBNull.Value;
            else if (dbColumn.DefaultValue == "getdate()" || dbColumn.DefaultValue == "(getdate())")
                return DateTime.Now.ToShortDateString();
            else if (dbColumn.DefaultValue == "newid()")
                return Guid.NewGuid().ToString();
            else
                return dbColumn.DefaultValue.Replace("(", "").Replace(")", "");
        }
    }
}