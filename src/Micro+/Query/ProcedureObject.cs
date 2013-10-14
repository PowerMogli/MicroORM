using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace MicroORM.Query
{
    public abstract class ProcedureObject : IEnumerable
    {
        private string _storedProcedureName = string.Empty;
        private Dictionary<string, IDataParameter> _parameters = new Dictionary<string, IDataParameter>();

        internal ProcedureObject(string storedProcedureName)
        {
            _storedProcedureName = storedProcedureName;
        }

        internal string StoredProcedureName { get { return _storedProcedureName; } }

        protected Dictionary<string, IDataParameter> Parameters { get { return _parameters; } }

        protected virtual bool AddParameter<T>(string parameterName, T value, DbType dbType, int length)
        {
            if (value is string && string.IsNullOrWhiteSpace(value.ToString())) throw new ArgumentNullException("value");
            if (string.IsNullOrWhiteSpace(parameterName)) throw new ArgumentNullException("parameterName");

            string stringValue = value as string;
            if (!string.IsNullOrWhiteSpace(stringValue)
                && length > 0
                && stringValue.Length > length) return false;

            return true;
        }

        protected T GetParameterValue<T>(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName)) throw new ArgumentNullException("parameterName");

            var parameter = this.Parameters[parameterName];
            if (parameter == null) return default(T);

            return (T)parameter.Value;
        }

        public IEnumerator GetEnumerator()
        {
            return this.Parameters.Values.GetEnumerator();
        }
    }
}
