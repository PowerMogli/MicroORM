// --------------------------------------------------------------------------------------------------------------------
// <copyright file="QueryParameterCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The query parameter collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Query
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data;

    using RabbitDB.Mapping;
    using RabbitDB.Query.StoredProcedure;
    using RabbitDB.Reflection;
    using RabbitDB.Utils;

    /// <summary>
    /// The query parameter collection.
    /// </summary>
    class QueryParameterCollection : Collection<QueryParameter>
    {
        #region Methods

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="QueryParameterCollection"/>.
        /// </returns>
        internal static QueryParameterCollection Create<T>(object[] arguments)
        {
            if (arguments == null)
            {
                return new QueryParameterCollection();
            }

            var tableInfo = TableInfo<T>.GetTableInfo;

            return Create(arguments, tableInfo);
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        /// <returns>
        /// The <see cref="QueryParameterCollection"/>.
        /// </returns>
        internal static QueryParameterCollection Create(object[] arguments, TableInfo tableInfo = null)
        {
            if (arguments == null)
            {
                return new QueryParameterCollection();
            }

            var collection = CreateParameterFromAnonymous(arguments, tableInfo);
            if (collection.Count != 0)
            {
                return collection;
            }

            collection = CreateParameterFromProcedureParameterCollection(arguments);

            return collection.Count != 0 ? collection : CreateParameterFromRegular(arguments);
        }

        /// <summary>
        /// The add range.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        internal void AddRange(object[] arguments)
        {
            var collection = Create(arguments);

            foreach (var queryParameter in collection)
            {
                this.Add(queryParameter);
            }
        }

        /// <summary>
        /// The create parameter from anonymous.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        /// <returns>
        /// The <see cref="QueryParameterCollection"/>.
        /// </returns>
        private static QueryParameterCollection CreateParameterFromAnonymous(
            ICollection<object> arguments, 
            TableInfo tableInfo)
        {
            var collection = new QueryParameterCollection();
            if (arguments.Count < 1)
            {
                return collection;
            }

            foreach (var argument in arguments)
            {
                if (argument == null)
                {
                    continue;
                }

                var namedArguments = argument as KeyValuePair<string, object>[];
                if (namedArguments == null && !argument.IsListParam() && argument.IsCustomObject())
                {
                    namedArguments = ParameterTypeDescriptor.ToKeyValuePairs(new[] { argument });
                }

                collection.AddRange(CreateParameterFromKeyValuePairs(namedArguments, tableInfo));
            }

            return collection;
        }

        /// <summary>
        /// The create parameter from key value pairs.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        /// <returns>
        /// The <see cref="QueryParameterCollection"/>.
        /// </returns>
        private static IEnumerable<QueryParameter> CreateParameterFromKeyValuePairs(
            IEnumerable<KeyValuePair<string, object>> argument, 
            TableInfo tableInfo)
        {
            var collection = new QueryParameterCollection();
            if (argument == null)
            {
                return collection;
            }

            foreach (var kvp in argument)
            {
                collection.Add(QueryParameter.CreateFromKeyValuePairs(kvp, tableInfo));
            }

            return collection;
        }

        /// <summary>
        /// The create parameter from procedure parameter collection.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="QueryParameterCollection"/>.
        /// </returns>
        private static QueryParameterCollection CreateParameterFromProcedureParameterCollection(IList<object> arguments)
        {
            if (arguments == null || arguments.Count == 0)
            {
                return new QueryParameterCollection();
            }

            var procedureCollection = arguments[0] as ProcedureParameterCollection;
            if (procedureCollection == null)
            {
                return new QueryParameterCollection();
            }

            var queryParameterCollection = new QueryParameterCollection();
            foreach (IDbDataParameter parameter in procedureCollection)
            {
                queryParameterCollection.Add(QueryParameter.CreateFromDbParameter(parameter));
            }

            return queryParameterCollection;
        }

        /// <summary>
        /// The create parameter from regular.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="QueryParameterCollection"/>.
        /// </returns>
        private static QueryParameterCollection CreateParameterFromRegular(object[] arguments)
        {
            var collection = new QueryParameterCollection();

            for (var index = 0; index < arguments.Length; index++)
            {
                collection.Add(QueryParameter.CreateFromRegular(index, arguments));
            }

            return collection;
        }

        /// <summary>
        /// The add range.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        private void AddRange(IEnumerable<QueryParameter> collection)
        {
            foreach (var queryParamter in collection)
            {
                Add(queryParamter);
            }
        }

        #endregion
    }
}