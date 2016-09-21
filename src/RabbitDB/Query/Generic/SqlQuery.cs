// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlQuery.cs" company="">
//   
// </copyright>
// <summary>
//   The sql query.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Data;

using RabbitDB.Contracts.SqlDialect;
using RabbitDB.Mapping;
using RabbitDB.SqlBuilder;

#endregion

namespace RabbitDB.Query.Generic
{
    /// <summary>
    ///     The sql query.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal sealed class SqlQuery<TEntity> : SqlQuery
    {
        #region Fields

        /// <summary>
        ///     The _additional predicate.
        /// </summary>
        private readonly string _additionalPredicate;

        /// <summary>
        ///     The _primary keys.
        /// </summary>
        private readonly object[] _primaryKeys;

        /// <summary>
        ///     The _table info.
        /// </summary>
        private readonly TableInfo _tableInfo;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlQuery{TEntity}" /> class.
        /// </summary>
        /// <param name="primaryKeys">
        ///     The primary keys.
        /// </param>
        /// <param name="additionalPredicate">
        ///     The additional predicate.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        internal SqlQuery(object[] primaryKeys, string additionalPredicate, QueryParameterCollection arguments = null)
            : base(string.Empty, arguments)
        {
            _tableInfo = TableInfo<TEntity>.GetTableInfo;
            _primaryKeys = primaryKeys;
            _additionalPredicate = additionalPredicate;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlQuery{TEntity}" /> class.
        /// </summary>
        /// <param name="sqlStatement">
        ///     The sql statement.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        internal SqlQuery(string sqlStatement, QueryParameterCollection arguments = null)
            : base(sqlStatement, arguments)
        {
            _tableInfo = TableInfo<TEntity>.GetTableInfo;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The compile.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        /// <returns>
        ///     The <see cref="IDbCommand" />.
        /// </returns>
        public override IDbCommand Compile(ISqlDialect sqlDialect)
        {
            if (_primaryKeys != null)
            {
                PrepareSqlStatement();
            }

            PrepareArguments();

            return base.Compile(sqlDialect);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The prepare arguments.
        /// </summary>
        /// <exception cref="PrimaryKeyException">
        /// </exception>
        private void PrepareArguments()
        {
            if (_primaryKeys == null || _primaryKeys.Length <= 0)
            {
                return;
            }

            if (_primaryKeys.Length != _tableInfo.NumberOfPrimaryKeys)
            {
                throw new PrimaryKeyException(
                    "The number of provided primaryKeys does not match the requested number of primaryKeys!");
            }

            if (Arguments == null)
            {
                Arguments = new QueryParameterCollection();
            }

            Arguments.AddRange(_primaryKeys);
        }

        /// <summary>
        ///     The prepare sql statement.
        /// </summary>
        private void PrepareSqlStatement()
        {
            Sql = SqlBuilder<TEntity>.SelectStatement;
            if (string.IsNullOrEmpty(_additionalPredicate))
            {
                return;
            }

            Sql = $"{Sql} and {_additionalPredicate}";
        }

        #endregion
    }
}