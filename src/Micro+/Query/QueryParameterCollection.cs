using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using MicroORM.Mapping;
using MicroORM.Reflection;
using MicroORM.Utils;
using MicroORM.Schema;

namespace MicroORM.Query
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

        internal static QueryParameterCollection Create(object[] arguments, TableInfo tableInfo = null)
        {
            if (arguments == null) return new QueryParameterCollection();

            QueryParameterCollection collection = CreateParameterFromAnonymous(arguments, tableInfo);
            if (collection.Count != 0) return collection;

            return CreateParameterFromRegular(arguments);
        }

        internal static QueryParameterCollection Create<T>(object[] arguments)
        {
            if (arguments == null) return new QueryParameterCollection();

            TableInfo tableInfo = TableInfo.GetTableInfo(typeof(T));

            return Create(arguments, tableInfo);
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

            var argument = arguments[0];
            if (argument != null)
            {
                if (!argument.IsListParam() && argument.IsCustomObject())
                {
                    KeyValuePair<string, object>[] resultSet = ParameterTypeDescriptor.ToKeyValuePairs(arguments);
                    foreach (KeyValuePair<string, object> kvp in resultSet)
                    {
                        collection.Add(new QueryParameter(kvp.Key, tableInfo != null ? tableInfo.ConvertToDbType(kvp.Key) : TypeConverter.ToDbType(kvp.Value.GetType()), kvp.Value));
                    }
                }
            }
            return collection;
        }
    }
}
