using System.Data;
using MicroORM.Query.Generic;
using MicroORM.Storage;

namespace MicroORM.Query
{
    internal class StoredProcedureQuery : IQuery
    {
        private ProcedureObject _procedureWorkObject;

        internal StoredProcedureQuery(ProcedureObject procedureWorkObject)
        {
            _procedureWorkObject = procedureWorkObject;
        }

        public IDbCommand Compile(IDbProvider provider)
        {
            IDbCommand command = provider.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = _procedureWorkObject.StoredProcedureName;
            SetArguments(command);

            return command;
        }

        private void SetArguments(IDbCommand command)
        {
            foreach (var parameter in _procedureWorkObject)
            {
                command.Parameters.Add(parameter);
            }
        }
    }
}
