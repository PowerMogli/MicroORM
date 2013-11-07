using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Linq;
using RabbitDB.Mapping;
using RabbitDB.Query.StoredProcedure;
using RabbitDB.Reflection;
using RabbitDB.Schema;
using RabbitDB.Utils;

namespace RabbitDB.Query
{
    class QueryParameterCollection : Collection<QueryParameter>
    {
        private static CultureInfo _culture = CultureInfo.InvariantCulture;

        internal void AddRange(object[] arguments)
        {
            QueryParameterCollection collection = Create(arguments);

            foreach (QueryParameter queryParameter in collection)
            {
                this.Add(queryParameter);
            }
        }

        private void AddRange(QueryParameterCollection collection)
        {
            foreach (QueryParameter queryParamter in collection)
            {
                this.Add(queryParamter);
            }
        }

        internal static QueryParameterCollection Create<T>(object[] arguments)
        {
            if (arguments == null) return new QueryParameterCollection();

            TableInfo tableInfo = TableInfo<T>.GetTableInfo;

            return Create(arguments, tableInfo);
        }

        internal static QueryParameterCollection Create(object[] arguments, TableInfo tableInfo = null)
        {
            if (arguments == null) return new QueryParameterCollection();

            QueryParameterCollection collection = CreateParameterFromAnonymous(arguments, tableInfo);
            if (collection.Count != 0) return collection;

            collection = CreateParameterFromProcedureParameterCollection(arguments);
            if (collection.Count != 0) return collection;

            return CreateParameterFromRegular(arguments);
        }

        private static QueryParameterCollection CreateParameterFromProcedureParameterCollection(object[] arguments)
        {
            if (arguments == null || arguments.Length == 0) return new QueryParameterCollection();
            ProcedureParameterCollection procedureCollection = arguments[0] as ProcedureParameterCollection;
            if (procedureCollection == null) return new QueryParameterCollection();

            QueryParameterCollection queryParameterCollection = new QueryParameterCollection();
            foreach (IDbDataParameter parameter in procedureCollection)
            {
                queryParameterCollection.Add(new QueryParameter(parameter.ParameterName, parameter.DbType, parameter.Value, parameter.Size));
            }
            return queryParameterCollection;
        }

        private static QueryParameterCollection CreateParameterFromRegular(object[] arguments)
        {
            QueryParameterCollection collection = new QueryParameterCollection();

            for (int i = 0; i < arguments.Length; i++)
            {
                Type argumentType = arguments[i].GetType();
                if (argumentType.IsEnum) argumentType = typeof(Int32);
                collection.Add(new QueryParameter(i.ToString(_culture), TypeConverter.ToDbType(argumentType), arguments[i]));
            }
            return collection;
        }

        private static QueryParameterCollection CreateParameterFromAnonymous(object[] arguments, TableInfo tableInfo)
        {
            QueryParameterCollection collection = new QueryParameterCollection();
            if (arguments.Length < 1) return collection;

            for (int index = 0; index < arguments.Length; index++)
            {
                var argument = arguments[index];
                if (argument != null)
                {
                    KeyValuePair<string, object>[] namedArguments = argument as KeyValuePair<string, object>[];
                    if (namedArguments == null && !argument.IsListParam() && argument.IsCustomObject())
                        namedArguments = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { argument });

                    collection.AddRange(CreateParameterFromKeyValuePairs(namedArguments, tableInfo));
                }
            }
            return collection;
        }

        private static QueryParameterCollection CreateParameterFromKeyValuePairs(KeyValuePair<string, object>[] argument, TableInfo tableInfo)
        {
            QueryParameterCollection collection = new QueryParameterCollection();
            if (argument == null) return collection;

            foreach (KeyValuePair<string, object> kvp in argument)
            {
                Type argumentType = null;
                if (kvp.Value != null)
                {
                    argumentType = kvp.Value.GetType();
                    if (argumentType.IsEnum) argumentType = typeof(Int32);
                }

                collection.Add(new QueryParameter(
                        kvp.Key,
                        tableInfo != null && tableInfo.IsColumn(kvp.Key) ? tableInfo.ConvertToDbType(kvp.Key) : TypeConverter.ToDbType(argumentType),
                        EvaluateParameterValue(tableInfo, kvp),
                        tableInfo != null && tableInfo.IsColumn(kvp.Key) ? tableInfo.GetColumnSize(kvp.Key) : -1));

                argumentType = null;
            }

            return collection;
        }

        private static object EvaluateParameterValue(TableInfo tableInfo, KeyValuePair<string, object> kvp)
        {
            if (tableInfo == null)
                return kvp.Value == null ? DBNull.Value : kvp.Value;

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