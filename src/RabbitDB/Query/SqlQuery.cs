using System.Data;
using RabbitDB.Storage;

namespace RabbitDB.Query
{
    internal class SqlQuery : IQuery, IArgumentQuery
    {
        protected string _sql;

        public string SqlStatement
        {
            get { return _sql; }
            internal set { _sql = value; }
        }
        public QueryParameterCollection Arguments { get; protected set; }

        internal SqlQuery(string sql, QueryParameterCollection arguments = null)
        {
            _sql = sql;
            this.Arguments = arguments;
        }

        public virtual IDbCommand Compile(IDbProvider provider)
        {
            DbCommandCompiler commandCompiler = new DbCommandCompiler(this, provider);
            IDbCommand dbCommand = commandCompiler.Compile();
            dbCommand.CommandType = CommandType.Text;

            return dbCommand;
        }
    }
}