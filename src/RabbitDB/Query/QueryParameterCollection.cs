// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryParameterCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The query parameter collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;

using RabbitDB.Contracts.Query;
using RabbitDB.Mapping;
using RabbitDB.Query.StoredProcedure;
using RabbitDB.Reflection;
using RabbitDB.Utils;

#endregion

namespace RabbitDB.Query
{
    /// <summary>
    ///     The query parameter collection.
    /// </summary>
    internal class QueryParameterCollection : Collection<QueryParameter>,
                                              IQueryParameterCollection<QueryParameter>
    {
        #region Internal Methods

        /// <summary>
        ///     The add range.
        /// </summary>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        public void AddRange(object[] arguments)
        {
            QueryParameterCollection collection = Create(arguments);

            foreach (QueryParameter queryParameter in collection)
            {
                Add(queryParameter);
            }
        }

        /// <summary>
        ///     The create.
        /// </summary>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="QueryParameterCollection" />.
        /// </returns>
        internal static QueryParameterCollection Create<T>(object[] arguments)
        {
            if (arguments == null)
            {
                return new QueryParameterCollection();
            }

            TableInfo tableInfo = TableInfo<T>.GetTableInfo;

            return Create(arguments, tableInfo);
        }

        /// <summary>
        ///     The create.
        /// </summary>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        /// <returns>
        ///     The <see cref="QueryParameterCollection" />.
        /// </returns>
        internal static QueryParameterCollection Create(object[] arguments, TableInfo tableInfo = null)
        {
            if (arguments == null)
            {
                return new QueryParameterCollection();
            }

            QueryParameterCollection collection = CreateParameterFromAnonymous(arguments, tableInfo);

            if (collection.Count != 0)
            {
                return collection;
            }

            collection = CreateParameterFromProcedureParameterCollection(arguments);

            return collection.Count != 0
                ? collection
                : CreateParameterFromRegular(arguments);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The add range.
        /// </summary>
        /// <param name="collection">
        ///     The collection.
        /// </param>
        private void AddRange(IEnumerable<QueryParameter> collection)
        {
            foreach (QueryParameter queryParamter in collection)
            {
                Add(queryParamter);
            }
        }

        /// <summary>
        ///     The create parameter from anonymous.
        /// </summary>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        /// <returns>
        ///     The <see cref="QueryParameterCollection" />.
        /// </returns>
        private static QueryParameterCollection CreateParameterFromAnonymous(
            ICollection<object> arguments,
            TableInfo tableInfo)
        {
            QueryParameterCollection collection = new QueryParameterCollection();
            if (arguments.Count < 1)
            {
                return collection;
            }

            foreach (object argument in arguments)
            {
                if (argument == null)
                {
                    continue;
                }

                KeyValuePair<string, object>[] namedArguments = argument as KeyValuePair<string, object>[];

                if (namedArguments == null && !argument.IsListParam() && argument.IsCustomObject())
                {
                    namedArguments = ParameterTypeDescriptor.ToKeyValuePairs(new[] { argument });
                }

                collection.AddRange(CreateParameterFromKeyValuePairs(namedArguments, tableInfo));
            }

            return collection;
        }

        /// <summary>
        ///     The create parameter from key value pairs.
        /// </summary>
        /// <param name="argument">
        ///     The argument.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        /// <returns>
        ///     The <see cref="QueryParameterCollection" />.
        /// </returns>
        private static IEnumerable<QueryParameter> CreateParameterFromKeyValuePairs(
            IEnumerable<KeyValuePair<string, object>> argument,
            TableInfo tableInfo)
        {
            QueryParameterCollection collection = new QueryParameterCollection();

            if (argument == null)
            {
                return collection;
            }

            foreach (KeyValuePair<string, object> kvp in argument)
            {
                collection.Add(QueryParameter.CreateFromKeyValuePairs(kvp, tableInfo));
            }

            return collection;
        }

        /// <summary>
        ///     The create parameter from procedure parameter collection.
        /// </summary>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="QueryParameterCollection" />.
        /// </returns>
        private static QueryParameterCollection CreateParameterFromProcedureParameterCollection(IList<object> arguments)
        {
            if (arguments == null || arguments.Count == 0)
            {
                return new QueryParameterCollection();
            }

            ProcedureParameterCollection procedureCollection = arguments[0] as ProcedureParameterCollection;

            if (procedureCollection == null)
            {
                return new QueryParameterCollection();
            }

            QueryParameterCollection queryParameterCollection = new QueryParameterCollection();

            foreach (IDbDataParameter parameter in procedureCollection)
            {
                queryParameterCollection.Add(QueryParameter.CreateFromDbParameter(parameter));
            }

            return queryParameterCollection;
        }

        /// <summary>
        ///     The create parameter from regular.
        /// </summary>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        /// <returns>
        ///     The <see cref="QueryParameterCollection" />.
        /// </returns>
        private static QueryParameterCollection CreateParameterFromRegular(object[] arguments)
        {
            QueryParameterCollection collection = new QueryParameterCollection();

            for (int index = 0; index < arguments.Length; index++)
            {
                collection.Add(QueryParameter.CreateFromRegular(index, arguments));
            }

            return collection;
        }

        #endregion
    }
}