using System.Data;
using MicroORM.Query.Generic;
using MicroORM.Storage;

namespace MicroORM.Query
{
    internal class StoredProcedureQuery : IQuery
    {
        private string _storedProcedureName;
        private ProcedureObject _parameterCollection;

        internal StoredProcedureQuery(ProcedureObject parameterCollection)
        {

        }

        public IDbCommand Compile(IDbProvider provider)
        {
            IDbCommand command = provider.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            SetArguments(command);

            return command;
        }

        private void SetArguments(IDbCommand command)
        {
            foreach (var parameter in _parameterCollection)
            {
                command.Parameters.Add(parameter);
            }
        }
    }
}
