using System.Collections.Generic;
using System.Data;
using MicroORM.Mapping;
using MicroORM.Storage;
using MicroORM.Base;

namespace MicroORM.Query.Generic
{
    internal sealed class SqlQuery<T> : SqlQuery
    {
        private object[] _primaryKeys = null;
        private string _additionalPredicate = null;
        private TableInfo _tableInfo = null;

        internal SqlQuery(object[] primaryKeys, string additionalPredicate = null, params object[] arguments)
            : base(string.Empty, arguments)
        {
            _tableInfo = TableInfo.GetTableInfo(typeof(T));
            _primaryKeys = primaryKeys;
            _additionalPredicate = additionalPredicate;
        }

        internal SqlQuery(string sqlStatement, params object[] arguments)
            : base(sqlStatement, arguments) { }

        public override IDbCommand Compile(IDbProvider provider)
        {
            if (_primaryKeys != null)
                PrepareSqlStatement(provider);
            PrepareArguments();

            return base.Compile(provider);
        }

        private void PrepareSqlStatement(IDbProvider provider)
        {
            base._sql = _tableInfo.CreateSelectStatement(provider);
            if (string.IsNullOrEmpty(_additionalPredicate)) return;

            base._sql = string.Format("{0} and {1}", base._sql, _additionalPredicate);
        }

        private void PrepareArguments()
        {
            List<object> arguments = new List<object>(base.Arguments);
            if (_primaryKeys != null && _primaryKeys.Length > 0)
            {
                if (_primaryKeys.Length != _tableInfo.NumberOfPrimaryKeys)
                    throw new PrimaryKeyException("The number of provided primaryKeys does not match the requested number of primaryKeys!");

                arguments.AddRange(_primaryKeys);
            }
            base._arguments = arguments.ToArray();
        }
    }
}
