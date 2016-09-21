// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="UpdateTableBuilder.cs">
//   
// </copyright>
// <summary>
//   The BuildUpdateTable interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Linq.Expressions;

using RabbitDB.Contracts.SqlDialect;
using RabbitDB.Mapping;
using RabbitDB.SqlBuilder;

#endregion

namespace RabbitDB.Expressions
{
    /// <summary>
    ///     The BuildUpdateTable interface.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal interface IBuildUpdateTable<T>
    {
        #region Public Methods

        /// <summary>
        ///     The set.
        /// </summary>
        /// <param name="column">
        ///     The column.
        /// </param>
        /// <param name="statement">
        ///     The statement.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>IBuildUpdateTable</cref>
        ///     </see>
        ///     .
        /// </returns>
        IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement);

        /// <summary>
        ///     The set.
        /// </summary>
        /// <param name="column">
        ///     The column.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>IBuildUpdateTable</cref>
        ///     </see>
        ///     .
        /// </returns>
        IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value);

        /// <summary>
        ///     The set.
        /// </summary>
        /// <param name="column">
        ///     The column.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        void Set(string column, object value);

        /// <summary>
        ///     The where.
        /// </summary>
        /// <param name="criteria">
        ///     The criteria.
        /// </param>
        void Where(Expression<Func<T, bool>> criteria);

        #endregion
    }

    /// <summary>
    ///     The update table builder.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class UpdateTableBuilder<T> : IBuildUpdateTable<T>
    {
        #region Fields

        /// <summary>
        ///     The _builder.
        /// </summary>
        private readonly SqlExpressionBuilder<T> _builder;

        /// <summary>
        ///     The _sql dialect.
        /// </summary>
        private readonly ISqlDialect _sqlDialect;

        /// <summary>
        ///     The _table info.
        /// </summary>
        private readonly TableInfo _tableInfo;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="UpdateTableBuilder{T}" /> class.
        /// </summary>
        /// <param name="sqlDialect">
        ///     The sql dialect.
        /// </param>
        /// <param name="updateSqlBuilder">
        ///     The update sql builder.
        /// </param>
        public UpdateTableBuilder(ISqlDialect sqlDialect, UpdateSqlBuilder updateSqlBuilder)
        {
            _builder = new SqlExpressionBuilder<T>(sqlDialect);
            _sqlDialect = sqlDialect;
            _tableInfo = TableInfo<T>.GetTableInfo;
            _builder.Append(updateSqlBuilder.GetBaseUpdate());
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The get parameters.
        /// </summary>
        /// <returns>
        ///     The <see cref="object[]" />.
        /// </returns>
        public object[] GetParameters()
        {
            return _builder.Parameters.ToArray();
        }

        /// <summary>
        ///     The get sql.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string GetSql()
        {
            return _builder.ToString();
        }

        /// <summary>
        ///     The set.
        /// </summary>
        /// <param name="column">
        ///     The column.
        /// </param>
        /// <param name="statement">
        ///     The statement.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>IBuildUpdateTable</cref>
        ///     </see>
        ///     .
        /// </returns>
        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement)
        {
            _builder.Append($" {_sqlDialect.SqlCharacters.EscapeName(_tableInfo.ResolveColumnName(column.Body.GetPropertyName()))}=");

            _builder.Write(statement);

            _builder.Append(",");

            return this;
        }

        /// <summary>
        ///     The set.
        /// </summary>
        /// <param name="column">
        ///     The column.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>IBuildUpdateTable</cref>
        ///     </see>
        ///     .
        /// </returns>
        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value)
        {
            Set(column.Body.GetPropertyName(), value);

            return this;
        }

        /// <summary>
        ///     The set.
        /// </summary>
        /// <param name="column">
        ///     The column.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public void Set(string column, object value)
        {
            _builder.Append($" {_sqlDialect.SqlCharacters.EscapeName(_tableInfo.ResolveColumnName(column))}=@{_builder.Parameters.NextIndex},");

            _builder.Parameters.Add(value);
        }

        /// <summary>
        ///     The where.
        /// </summary>
        /// <param name="criteria">
        ///     The criteria.
        /// </param>
        public void Where(Expression<Func<T, bool>> criteria)
        {
            _builder.EndEnumeration();

            _builder.Where(criteria);
        }

        #endregion
    }
}