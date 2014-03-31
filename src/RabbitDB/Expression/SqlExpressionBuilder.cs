/*
 * Parts of the code used in this class 
 * originate from the OS project SqlFu:
 * https://github.com/sapiens/SqlFu
 * Licence used: Apache Licence v2.0
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * If anyone feels offended by the use of this code
 * please feel free to send me an e-Mail: albix@gmx.net
 */


using RabbitDB.Mapping;
using RabbitDB.SqlBuilder;
using RabbitDB.Storage;
using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;

namespace RabbitDB.Expressions
{
    internal class SqlExpressionBuilder<T>
    {
        private readonly SqlDialect.SqlDialect _sqlDialect;
        private readonly TableInfo _tableInfo;
        private StringBuilder _sqlQuery = new StringBuilder();
        private readonly ExpressionWriter<T> _expressionWriter;

        internal SqlExpressionBuilder(SqlDialect.SqlDialect sqlDialect)
        {
            _sqlDialect = sqlDialect;
            _tableInfo = TableInfo<T>.GetTableInfo;
            _expressionWriter = new ExpressionWriter<T>(sqlDialect.BuilderHelper, this.Parameters);
        }

        private static string SortToString(SortOrder sort)
        {
            return sort == SortOrder.Descending ? "DESC" : "ASC";
        }

        internal SqlExpressionBuilder<T> OrderBy(Expression<Func<T, object>> selector, SortOrder sort = SortOrder.Ascending)
        {
            if (_order)
            {
                _sqlQuery.Append(", ");
            }
            else
            {
                _sqlQuery.Append(" ORDER BY ");
            }

            var column = selector.Body.GetPropertyName();
            _sqlQuery.AppendFormat("{0} {1}", _sqlDialect.SqlCharacters.EscapeName(column), SortToString(sort));
            _order = true;
            return this;
        }

        internal SqlExpressionBuilder<T> EndEnumeration()
        {
            string query = _sqlQuery.ToString();
            int commaIndex = query.LastIndexOf(',');
            if (commaIndex < 0) return this;

            _sqlQuery = new StringBuilder(query.Substring(0, commaIndex));
            return this;
        }

        internal void Append(string text)
        {
            _sqlQuery.Append(text);
        }

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

        private bool _where;
        private bool _hasColumn;
        private bool _order;

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

        internal void CreateSelect(Expression<Func<T, bool>> criteria)
        {
            WriteSelectAllColumns();
            Where(criteria);
        }

        private void WriteSelectAllColumns()
        {
            var sqlBuilder = new SelectSqlBuilder(_sqlDialect, _tableInfo);
            _sqlQuery.Append(sqlBuilder.GetBaseSelect());
        }

        internal void Write(Expression<Func<T, bool>> criteria)
        {
            _sqlQuery.Append(_expressionWriter.Write(criteria));
        }

        internal void Write(Expression<Func<T, object>> statement)
        {
            _sqlQuery.Append(_expressionWriter.Write(statement));
        }

        private ExpressionParameterCollection _parameterCollection;
        internal ExpressionParameterCollection Parameters
        {
            get { return _parameterCollection ?? (_parameterCollection = new ExpressionParameterCollection()); }
        }

        public override string ToString()
        {
            return _sqlQuery.ToString();
        }
    }
}
