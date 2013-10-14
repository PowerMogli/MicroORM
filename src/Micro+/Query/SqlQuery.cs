using System.Data;
using MicroORM.Storage;

namespace MicroORM.Query
{
    internal class SqlQuery : IQuery, IArgumentQuery
    {
        protected string _sql;
        protected object[] _arguments;

        public string SqlStatement { get { return _sql; } set { _sql = value; } }
        public object[] Arguments { get { return _arguments; } }

        internal SqlQuery() { }

        internal SqlQuery(string sql, params object[] arguments)
        {
            _sql = sql;
            _arguments = arguments;
        }

        public virtual IDbCommand Compile(IDbProvider provider)
        {
            IDbCommand command = provider.CreateCommand();
            command.CommandType = CommandType.Text;
            QueryInterpreter queryInterpreter = new QueryInterpreter(this, provider);
            queryInterpreter.Setup(command);

            return command;
        }
    }
}
