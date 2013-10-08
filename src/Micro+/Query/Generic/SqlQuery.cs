using System.Collections.Generic;
using System.Data;
using MicroORM.Mapping;
using MicroORM.Storage;

namespace MicroORM.Query.Generic
{
    internal sealed class SqlQuery<T> : SqlQuery
    {
        private object _primaryKey = null;
        private string _additionalPredicate = null;

        internal SqlQuery(object primaryKey, string additionalPredicate = null, params object[] arguments)
            : base(string.Empty, arguments)
        {
            _primaryKey = primaryKey;
            _additionalPredicate = additionalPredicate;
        }

        internal SqlQuery(string sqlStatement, params object[] arguments)
            : base(sqlStatement, arguments) { }

        public override IDbCommand Compile(IDbProvider provider)
        {
            PrepareSqlStatement(provider);
            PrepareArguments();

            return base.Compile(provider);
        }

        private void PrepareSqlStatement(IDbProvider provider)
        {
            TableInfo typeMapping = TableInfo.GetTableInfo(typeof(T));
            base._sql = typeMapping.CreateSelectStatement(provider);
            if (string.IsNullOrEmpty(_additionalPredicate)) return;

            base._sql = string.Format("{0} and {1}", base._sql, _additionalPredicate);
        }

        private void PrepareArguments()
        {
            List<object> arguments = new List<object>(base.Arguments);
            arguments.Add(_primaryKey);
            base._arguments = arguments.ToArray();
        }
    }
}
