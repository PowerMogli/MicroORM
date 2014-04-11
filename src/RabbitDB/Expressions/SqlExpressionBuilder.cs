// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="SqlExpressionBuilder.cs">
//   
// </copyright>
// <summary>
//   The sql expression builder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Expressions
{
    using System;
    using System.Data.SqlClient;
    using System.Linq.Expressions;
    using System.Text;

    using RabbitDB.Mapping;
    using RabbitDB.SqlBuilder;
    using RabbitDB.SqlDialect;

    /// <summary>
    /// The sql expression builder.
    /// </summary>
    /// <typeparam name="T">
    /// </typeparam>
    internal class SqlExpressionBuilder<T>
    {
        #region Fields

        /// <summary>
        /// The _expression writer.
        /// </summary>
        private readonly ExpressionWriter<T> _expressionWriter;

        /// <summary>
        /// The _sql dialect.
        /// </summary>
        private readonly SqlDialect _sqlDialect;

        /// <summary>
        /// The _table info.
        /// </summary>
        private readonly TableInfo _tableInfo;

        /// <summary>
        /// The _has column.
        /// </summary>
        private bool _hasColumn;

        /// <summary>
        /// The _order.
        /// </summary>
        private bool _order;

        /// <summary>
        /// The _parameter collection.
        /// </summary>
        private ExpressionParameterCollection _parameterCollection;

        /// <summary>
        /// The _sql query.
        /// </summary>
        private StringBuilder _sqlQuery = new StringBuilder();

        /// <summary>
        /// The _where.
        /// </summary>
        private bool _where;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlExpressionBuilder{T}"/> class.
        /// </summary>
        /// <param name="sqlDialect">
        /// The sql dialect.
        /// </param>
        internal SqlExpressionBuilder(SqlDialect sqlDialect)
        {
            _sqlDialect = sqlDialect;
            _tableInfo = TableInfo<T>.GetTableInfo;
            _expressionWriter = new ExpressionWriter<T>(sqlDialect.BuilderHelper, this.Parameters);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        internal ExpressionParameterCollection Parameters
        {
            get
            {
                return _parameterCollection ?? (_parameterCollection = new ExpressionParameterCollection());
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            return _sqlQuery.ToString();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The append.
        /// </summary>
        /// <param name="text">
        /// The text.
        /// </param>
        internal void Append(string text)
        {
            _sqlQuery.Append(text);
        }

        /// <summary>
        /// The create select.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        internal void CreateSelect(Expression<Func<T, bool>> criteria)
        {
            WriteSelectAllColumns();
            Where(criteria);
        }

        /// <summary>
        /// The end enumeration.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>SqlExpressionBuilder</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal SqlExpressionBuilder<T> EndEnumeration()
        {
            var query = _sqlQuery.ToString();
            var commaIndex = query.LastIndexOf(',');

            if (commaIndex < 0)
            {
                return this;
            }

            _sqlQuery = new StringBuilder(query.Substring(0, commaIndex));

            return this;
        }

        /// <summary>
        /// The order by.
        /// </summary>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <param name="sort">
        /// The sort.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>SqlExpressionBuilder</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal SqlExpressionBuilder<T> OrderBy(
            Expression<Func<T, object>> selector, 
            SortOrder sort = SortOrder.Ascending)
        {
            _sqlQuery.Append(_order ? ", " : " ORDER BY ");

            var column = selector.Body.GetPropertyName();
            _sqlQuery.AppendFormat(
                "{0} {1}", 
                _sqlDialect.SqlCharacters.EscapeName(column), 
                SortToString(sort));

            _order = true;

            return this;
        }

        /// <summary>
        /// The where.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>SqlExpressionBuilder</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal SqlExpressionBuilder<T> Where(Expression<Func<T, bool>> criteria)
        {
            if (!_where)
            {
                _sqlQuery.Append(" WHERE ");
                _where = true;
            }
            else
            {
                _sqlQuery.Append(" AND ");
            }

            Write(criteria);

            return this;
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="criteria">
        /// The criteria.
        /// </param>
        internal void Write(Expression<Func<T, bool>> criteria)
        {
            _sqlQuery.Append(_expressionWriter.Write(criteria));
        }

        /// <summary>
        /// The write.
        /// </summary>
        /// <param name="statement">
        /// The statement.
        /// </param>
        internal void Write(Expression<Func<T, object>> statement)
        {
            _sqlQuery.Append(_expressionWriter.Write(statement));
        }

        /// <summary>
        /// The write select column.
        /// </summary>
        /// <param name="selector">
        /// The selector.
        /// </param>
        /// <param name="alias">
        /// The alias.
        /// </param>
        /// <typeparam name="R">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>SqlExpressionBuilder</cref>
        ///     </see>
        ///     .
        /// </returns>
        internal SqlExpressionBuilder<T> WriteSelectColumn<R>(Expression<Func<T, R>> selector, string alias = null)
        {
            var name = selector.Body.GetPropertyName();
            if (_hasColumn)
            {
                _sqlQuery.Append(", ");
            }

            _sqlQuery.Append(_sqlDialect.SqlCharacters.EscapeName(_tableInfo.ResolveColumnName(name)));
            _hasColumn = true;

            if (alias != null)
            {
                _sqlQuery.AppendFormat(" AS {0}", alias);
            }

            return this;
        }

        /// <summary>
        /// The sort to string.
        /// </summary>
        /// <param name="sort">
        /// The sort.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string SortToString(SortOrder sort)
        {
            return sort == SortOrder.Descending ? "DESC" : "ASC";
        }

        /// <summary>
        /// The write select all columns.
        /// </summary>
        private void WriteSelectAllColumns()
        {
            var sqlBuilder = new SelectSqlBuilder(_sqlDialect, _tableInfo);
            _sqlQuery.Append(sqlBuilder.GetBaseSelect());
        }

        #endregion
    }
}