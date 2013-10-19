using System.Data;
using MicroORM.Storage;

namespace MicroORM.Query
{
    internal class QueryInterpreter
    {
        private IArgumentQuery _query;
        private IDbProvider _provider;

        internal QueryInterpreter(IArgumentQuery query, IDbProvider provider)
        {
            _query = query;
            _provider = provider;
        }

        internal void Setup(IDbCommand command)
        {
            SetupParameter(command);
            command.CommandText = _query.SqlStatement;
        }

        private void SetupParameter(IDbCommand command)
        {
            if (_query.Arguments == null) return;

            foreach (QueryParameter argument in _query.Arguments)
            {
                IDbDataParameter parameter = command.CreateParameter();
                parameter.Setup(argument, _provider.ParameterPrefix);

                command.Parameters.Add(parameter);
            }
        }
    }
}
