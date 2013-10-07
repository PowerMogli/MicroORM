using System;
using System.Data.SqlClient;

namespace MicroORM.Query.Generic
{
    public class SqlProcedureObject<T> : ProcedureObject where T : class, SqlParameter, new()
    {
        public SqlProcedureObject(string storedProcedureName)
            : base(storedProcedureName) { }

        protected override bool AddParameter<V>(string parameterName, V value, System.Data.SqlDbType dbType, int length = -1)
        {
            if (value is string && string.IsNullOrWhiteSpace(value.ToString())) throw new ArgumentNullException("value");

            string stringValue = value as string;
            if (!string.IsNullOrWhiteSpace(stringValue)
                && length > 0
                && stringValue.Length > length) return false;

            var parameter = new T() { ParameterName = parameterName, SqlDbType = dbType, Value = value }; ;
            if (length > 0)
                parameter.Size = length;

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
