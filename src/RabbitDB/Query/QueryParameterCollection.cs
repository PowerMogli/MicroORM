using RabbitDB.Mapping;
using RabbitDB.Query.StoredProcedure;
using RabbitDB.Reflection;
using RabbitDB.Utils;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

namespace RabbitDB.Query
{
    class QueryParameterCollection : Collection<QueryParameter>
    {
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
                queryParameterCollection.Add(QueryParameter.CreateFromDbParameter(parameter));
            }
            return queryParameterCollection;
        }

        private static QueryParameterCollection CreateParameterFromRegular(object[] arguments)
        {
            QueryParameterCollection collection = new QueryParameterCollection();

            for (int index = 0; index < arguments.Length; index++)
            {
                collection.Add(QueryParameter.CreateFromRegular(index, arguments));
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
                collection.Add(QueryParameter.CreateFromKeyValuePairs(kvp, tableInfo));
            }

            return collection;
        }
    }
}