using System.Data;
using MicroORM.Storage;

namespace MicroORM.Query
{
    internal class SqlQuery : IQuery, IArgumentQuery
    {
        protected string _sql;

        public string SqlStatement { get { return _sql; } internal set { _sql = value; } }
        public QueryParameterCollection Arguments { get; private set; }

        internal SqlQuery() { }

        internal SqlQuery(string sql, QueryParameterCollection arguments = null)
        {
            _sql = sql;
            this.Arguments = arguments;
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
