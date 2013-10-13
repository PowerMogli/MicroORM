using System.Collections.Generic;
using System.Data;
using System.Globalization;
using MicroORM.Reflection;
using MicroORM.Storage;
using MicroORM.Utils;

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
            var arguments = CreateParamsDictionary(_query.Arguments);

            foreach (var argument in arguments)
            {
                IDbDataParameter parameter = command.CreateParameter();
                _provider.SetupParameter(parameter, argument.Key, argument.Value);

                command.Parameters.Add(parameter);
            }
        }

        private static CultureInfo _culture = CultureInfo.InvariantCulture;
        private KeyValuePair<string, object>[] CreateParamsDictionary(object[] arguments)
        {
            var keyValuePairs=new KeyValuePair<string, object>[arguments.Length];
            if (arguments == null) return keyValuePairs;

            keyValuePairs = CreateParameterFromAnonymous(arguments);
            if (keyValuePairs.Length != 0) return keyValuePairs;

            return CreateParameterFromRegular(arguments);
        }

        private KeyValuePair<string, object>[] CreateParameterFromRegular(object[] arguments)
        {
            var keyValuePairs=new KeyValuePair<string, object>[arguments.Length];
            for (int i = 0; i < arguments.Length; i++)
            {
                keyValuePairs[i] = new KeyValuePair<string, object>(i.ToString(_culture), arguments[i]);
            }
            return keyValuePairs;
        }

        private KeyValuePair<string, object>[] CreateParameterFromAnonymous(object[] arguments)
        {
            if (arguments.Length == 1)
            {
                var argument = arguments[0];
                if (argument != null)
                {
                    if (!argument.IsListParam() && argument.IsCustomObject())
                    {
                        return ParameterTypeDescriptor.ToKeyValuePairs(arguments);
                    }
                }
            }
            return new KeyValuePair<string, object>[0];
        }
    }
}
