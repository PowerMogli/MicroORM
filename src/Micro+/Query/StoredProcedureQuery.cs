using System.Data;
using MicroORM.Storage;

namespace MicroORM.Query
{
    internal class StoredProcedureQuery : IQuery, IArgumentQuery
    {
        private StoredProcedure _procedureWorkObject;

        public QueryParameterCollection Arguments { get; private set; }
        public string SqlStatement { get; private set; }

        internal StoredProcedureQuery(StoredProcedure procedureWorkObject)
        {
            _procedureWorkObject = procedureWorkObject;
        }

        internal StoredProcedureQuery(string storedProcedureName, QueryParameterCollection arguments = null)
        {
            this.SqlStatement = storedProcedureName;
            this.Arguments = arguments;
        }

        public IDbCommand Compile(IDbProvider provider)
        {
            IDbCommand command = provider.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;

            return Compile(command, provider);
        }

        private IDbCommand Compile(IDbCommand command, IDbProvider provider)
        {
            if (_procedureWorkObject != null)
                return CompileWithProcedureObject(command);

            return CompileWithArguments(command, provider);
        }

        private IDbCommand CompileWithProcedureObject(IDbCommand command)
        {
            command.CommandText = _procedureWorkObject.StoredProcedureName;
            SetArguments(command);

            return command;
        }

        private IDbCommand CompileWithArguments(IDbCommand command, IDbProvider provider)
        {
            QueryInterpreter sqlQueryInterpreter = new QueryInterpreter(this, provider);
            sqlQueryInterpreter.Setup(command);

            return command;
        }

        private void SetArguments(IDbCommand command)
        {
            foreach (var parameter in _procedureWorkObject.Parameters)
            {
                command.Parameters.Add(parameter);
            }
        }
    }
}
