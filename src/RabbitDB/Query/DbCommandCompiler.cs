using System.Data;
using RabbitDB.Storage;

namespace RabbitDB.Query
{
    internal class DbCommandCompiler
    {
        private IArgumentQuery _query;
        private IDbProvider _provider;
        private IDbCommand _command;

        internal DbCommandCompiler(IArgumentQuery query, IDbProvider provider)
        {
            _query = query;
            _provider = provider;
            _command = provider.CreateCommand();
        }

        internal IDbCommand Compile()
        {
            SetupParameter();
            _command.CommandText = _query.SqlStatement;

            return _command;
        }

        private void SetupParameter()
        {
            if (_query.Arguments == null) return;

            foreach (QueryParameter argument in _query.Arguments)
            {
                IDbDataParameter parameter = _command.CreateParameter();
                parameter.Setup(argument, _provider.ParameterPrefix);

                _command.Parameters.Add(parameter);
            }
        }
    }
}
