using System.Collections.Generic;
using System.Data;
using MicroORM.Base.Reflection;
using MicroORM.Base.Storage;
using MicroORM.Base.Utils;
using System.Globalization;
using MicroORM.Base.Mapping;

namespace MicroORM.Base.Query
{
    internal class SqlQueryInterpreter
    {
        private SqlQuery _query;
        private IDbProvider _provider;

        internal SqlQueryInterpreter(SqlQuery query, IDbProvider provider)
        {
            _query = query;
            _provider = provider;
        }

        internal void Setup(IDbCommand command)
        {
            SetupParameter(command);
            command.CommandText = _query.SqlStatement;
            command.CommandType = CommandType.Text;
        }

        private void SetupParameter(IDbCommand command)
        {
            var arguments = CreateParamsDictionary(_query.Arguments);

            foreach (var argument in arguments)
            {
                IDbDataParameter parameter = command.CreateParameter();
                _provider.SetupParameter(parameter, argument.Key, argument.Value);

                command.Parameters.Add(parameter);
            }
        }

        private static CultureInfo culture = CultureInfo.InvariantCulture;
        private static KeyValuePair<string, object>[] CreateParamsDictionary(object[] args)
        {
            var keyValuePairs=new KeyValuePair<string, object>[args.Length];
            if (args == null) return keyValuePairs;

            keyValuePairs = CreateParameterFromAnonymous(args);
            if (keyValuePairs.Length != 0) return keyValuePairs;

            return CreateParameterFromRegular(args);
        }

        private static KeyValuePair<string, object>[] CreateParameterFromRegular(object[] args)
        {
            var keyValuePairs=new KeyValuePair<string, object>[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                keyValuePairs[i] = new KeyValuePair<string, object>(i.ToString(culture), args[i]);
            }
            return keyValuePairs;
        }

        private static KeyValuePair<string, object>[] CreateParameterFromAnonymous(object[] args)
        {
            if (args.Length == 1)
            {
                var poco = args[0];
                if (poco != null)
                {
                    if (!poco.IsListParam() && poco.IsCustomObject())
                    {
                        return ParameterTypeDescriptor.ToKeyValuePairs(args);
                    }
                }
            }
            return new KeyValuePair<string, object>[0];
        }
    }
}
