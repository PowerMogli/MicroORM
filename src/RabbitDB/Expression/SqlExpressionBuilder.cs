using System;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Text;
using RabbitDB.Mapping;
using RabbitDB.Storage;

namespace RabbitDB.Expressions
{
    internal class SqlExpressionBuilder<T>
    {
        private readonly IDbProvider _dbProvider;
        private readonly TableInfo _tableInfo;
        private readonly StringBuilder _sqlBuilder = new StringBuilder();
        private readonly ExpressionWriter<T> _expressionWriter;

        internal SqlExpressionBuilder(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
            _tableInfo = TableInfo<T>.GetTableInfo;
            _expressionWriter = new ExpressionWriter<T>(dbProvider.BuilderHelper, this.Parameters);
        }

        private static string SortToString(SortOrder sort)
        {
            return sort == SortOrder.Descending ? "desc" : "asc";
        }

        internal SqlExpressionBuilder<T> OrderBy(Expression<Func<T, object>> selector, SortOrder sort = SortOrder.Ascending)
        {
            if (_order)
            {
                _sqlBuilder.Append(", ");
            }
            else
            {
                _sqlBuilder.Append(" order by ");
            }

            var column = selector.Body.GetPropertyName();
            _sqlBuilder.AppendFormat("{0} {1}", _dbProvider.EscapeName(column), SortToString(sort));
            _order = true;
            return this;
        }

        internal void Append(string text)
        {
            _sqlBuilder.Append(text);
        }

        internal SqlExpressionBuilder<T> WriteSelectColumn<R>(Expression<Func<T, R>> selector, string alias = null)
        {
            var name = selector.Body.GetPropertyName();
            if (_hasColumn)
            {
                _sqlBuilder.Append(", ");
            }
            _sqlBuilder.Append(_dbProvider.EscapeName(_tableInfo.ResolveColumnName(name)));
            _hasColumn = true;
            if (alias != null)
            {
                _sqlBuilder.AppendFormat(" as {0}", alias);
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
                _sqlBuilder.Append(" where ");
                _where = true;
            }
            else
            {
                _sqlBuilder.Append(" and ");
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
            _sqlBuilder.Append(_tableInfo.GetBaseSelect(_dbProvider));
        }

        internal void Write(Expression<Func<T, bool>> criteria)
        {
            _sqlBuilder.Append(_expressionWriter.Write(criteria));
        }

        internal void Write(Expression<Func<T, object>> statement)
        {
            _sqlBuilder.Append(_expressionWriter.Write(statement));
        }

        internal ExpressionParameterCollection Parameters { get; private set; }

        public override string ToString()
        {
            return _sqlBuilder.ToString();
        }
    }
}
