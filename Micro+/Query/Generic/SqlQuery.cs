using MicroORM.Base.Mapping;
using MicroORM.Base.Storage;
using System.Data;
using System.Text;
using System.Collections.Generic;

namespace MicroORM.Base.Query.Generic
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

        public override IDbCommand Compile(IDbProvider provider)
        {
            IDbCommand command = provider.CreateCommand();
            PrepareSqlStatement(provider);
            PrepareArguments();
            new SqlQueryInterpreter(this, provider).Setup(command);

            return command;
        }

        private void PrepareSqlStatement(IDbProvider provider)
        {
            TypeMapping typeMapping = TypeMapping.GetTypeMapping(typeof(T));
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
