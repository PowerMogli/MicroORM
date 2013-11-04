using System.Data;
using RabbitDB.Storage;

namespace RabbitDB.Query.StoredProcedure
{
    internal class StoredProcedureQuery : IQuery, IArgumentQuery
    {
        public QueryParameterCollection Arguments { get; private set; }
        public string SqlStatement { get; private set; }

        internal StoredProcedureQuery(StoredProcedure procedureWorkObject)
        {
            this.SqlStatement = procedureWorkObject.StoredProcedureName;
            this.Arguments = QueryParameterCollection.Create(new object[] { procedureWorkObject.Parameters });
        }

        internal StoredProcedureQuery(string storedProcedureName, QueryParameterCollection arguments = null)
        {
            this.SqlStatement = storedProcedureName;
            this.Arguments = arguments;
        }

        public IDbCommand Compile(IDbProvider dbProvider)
        {
            DbCommandCompiler commandCompiler = new DbCommandCompiler(this, dbProvider);
            IDbCommand dbCommand = commandCompiler.Compile();
            dbCommand.CommandType = CommandType.StoredProcedure;

            return dbCommand;
        }
    }
}
