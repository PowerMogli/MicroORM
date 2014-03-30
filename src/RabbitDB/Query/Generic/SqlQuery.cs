using RabbitDB.Base;
using RabbitDB.Mapping;
using RabbitDB.SqlBuilder;
using RabbitDB.Storage;
using System.Data;

namespace RabbitDB.Query.Generic
{
    internal sealed class SqlQuery<TEntity> : SqlQuery
    {
        private object[] _primaryKeys = null;
        private string _additionalPredicate = null;
        private TableInfo _tableInfo = null;

        internal SqlQuery(object[] primaryKeys, string additionalPredicate, QueryParameterCollection arguments = null)
            : base(string.Empty, arguments)
        {
            _tableInfo = TableInfo<TEntity>.GetTableInfo;
            _primaryKeys = primaryKeys;
            _additionalPredicate = additionalPredicate;
        }

        internal SqlQuery(string sqlStatement, QueryParameterCollection arguments = null)
            : base(sqlStatement, arguments)
        {
            _tableInfo = TableInfo<TEntity>.GetTableInfo;
        }

        public override IDbCommand Compile(IDbProvider provider)
        {
            if (_primaryKeys != null)
                PrepareSqlStatement();
            PrepareArguments();

            return base.Compile(provider);
        }

        private void PrepareSqlStatement()
        {
            base._sql = SqlBuilder<TEntity>.SelectStatement;
            if (string.IsNullOrEmpty(_additionalPredicate)) return;

            base._sql = string.Format("{0} and {1}", base._sql, _additionalPredicate);
        }

        private void PrepareArguments()
        {
            if (_primaryKeys == null || _primaryKeys.Length <= 0) return;

            if (_primaryKeys.Length != _tableInfo.NumberOfPrimaryKeys)
                throw new PrimaryKeyException("The number of provided primaryKeys does not match the requested number of primaryKeys!");

            if (base.Arguments == null)
                base.Arguments = new QueryParameterCollection();

            base.Arguments.AddRange(_primaryKeys);
        }
    }
}