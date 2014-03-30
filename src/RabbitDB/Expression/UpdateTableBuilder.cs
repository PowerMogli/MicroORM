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

using System;
using System.Linq.Expressions;
using RabbitDB.Mapping;
using RabbitDB.Storage;
using RabbitDB.SqlBuilder;

namespace RabbitDB.Expressions
{
    internal interface IBuildUpdateTable<T>
    {
        IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement);
        IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value);
        void Set(string column, object value);
        void Where(Expression<Func<T, bool>> criteria);
    }

    internal class UpdateTableBuilder<T> : IBuildUpdateTable<T>
    {
        private readonly SqlExpressionBuilder<T> _builder;
        private TableInfo _tableInfo;
        private IDbProvider _dbProvider;
        private UpdateSqlBuilder _updateSqlBuilder;

        public UpdateTableBuilder(IDbProvider dbProvider)
        {
            _builder = new SqlExpressionBuilder<T>(dbProvider);
            _dbProvider = dbProvider;
            _tableInfo = TableInfo<T>.GetTableInfo;
            _updateSqlBuilder = new UpdateSqlBuilder(_dbProvider, _tableInfo);
            _builder.Append(_updateSqlBuilder.GetBaseUpdate());
        }

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, Expression<Func<T, object>> statement)
        {
            _builder.Append(string.Format(" {0}=", _dbProvider.EscapeName(_tableInfo.ResolveColumnName(column.Body.GetPropertyName()))));
            _builder.Write(statement);
            _builder.Append(",");
            return this;
        }

        public IBuildUpdateTable<T> Set(Expression<Func<T, object>> column, object value)
        {
            Set(column.Body.GetPropertyName(), value);
            return this;
        }

        public void Set(string column, object value)
        {
            _builder.Append(string.Format(" {0}=@{1},", _dbProvider.EscapeName(_tableInfo.ResolveColumnName(column)), _builder.Parameters.NextIndex));
            _builder.Parameters.Add(value);
        }

        public void Where(Expression<Func<T, bool>> criteria)
        {
            _builder.EndEnumeration();
            _builder.Where(criteria);
        }

        public string GetSql()
        {
            return _builder.ToString();
        }

        public object[] GetParameters()
        {
            return _builder.Parameters.ToArray();
        }
    }
}