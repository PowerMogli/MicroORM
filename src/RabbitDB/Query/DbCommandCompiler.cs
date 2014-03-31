using System.Data;

namespace RabbitDB.Query
{
    internal class DbCommandCompiler
    {
        private IArgumentQuery _query;
        private SqlDialect.SqlDialect _sqlDialect;
        private IDbCommand _command;

        internal DbCommandCompiler(IArgumentQuery query, SqlDialect.SqlDialect sqlDialect)
        {
            _query = query;
            _sqlDialect = sqlDialect;
            _command = sqlDialect.DbProvider.CreateCommand();
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
                parameter.Setup(argument, _sqlDialect.SqlCharacters.ParameterPrefix);

                _command.Parameters.Add(parameter);
            }
        }
    }
}