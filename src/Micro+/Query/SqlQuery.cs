using System.Data;
using MicroORM.Storage;

namespace MicroORM.Query
{
    internal class SqlQuery : IQuery
    {
        protected string _sql;
        protected object[] _arguments;

        public string SqlStatement { get { return _sql; } }
        public object[] Arguments { get { return _arguments; } }

        internal SqlQuery() { }

        internal SqlQuery(string sql, object[] arguments)
        {
            _sql = sql;
            _arguments = arguments;
        }

        public virtual IDbCommand Compile(IDbProvider provider)
        {
            IDbCommand command = provider.CreateCommand();
            new SqlQueryInterpreter(this, provider).Setup(command);

            return command;
        }
    }
}
