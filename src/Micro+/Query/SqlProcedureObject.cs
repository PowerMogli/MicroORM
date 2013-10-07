using System;
using System.Data;
using System.Data.SqlClient;

namespace MicroORM.Query
{
    public class SqlProcedureObject : ProcedureObject
    {
        public SqlProcedureObject(string storedProcedureName)
            : base(storedProcedureName) { }

        protected override bool AddParameter<T>(string parameterName, T value, DbType dbType, int length = -1)
        {
            if (value is string && string.IsNullOrWhiteSpace(value.ToString())) throw new ArgumentNullException("value");

            string stringValue = value as string;
            if (!string.IsNullOrWhiteSpace(stringValue)
                && length > 0
                && stringValue.Length > length) return false;

            SqlParameter parameter = new SqlParameter(parameterName, value) { DbType = dbType };
            if (length > 0)
                parameter.Size = length;

            base.Parameters.Add(parameterName, parameter);
            return true;
        }

        protected override T GetParameterValue<T>(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName)) throw new ArgumentNullException("parameterName");

            var parameter = base.Parameters[parameterName];
            if (parameter == null) return default(T);

            return (T)parameter.Value;
        }
    }
}
