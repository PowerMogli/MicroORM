using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace MicroORM.Query
{
    internal class ProcedureParameterCollection : IEnumerable<IDbDataParameter>
    {
        internal ProcedureParameterCollection()
        {
            this.Parameters = new Dictionary<string, IDbDataParameter>();
        }

        internal Dictionary<string, IDbDataParameter> Parameters { get; private set; }

        internal bool AddParameter<T>(string parameterName, T value, DbType dbType, int length)
        {
            if (value is string && string.IsNullOrWhiteSpace(value.ToString())) throw new ArgumentNullException("value");
            if (string.IsNullOrWhiteSpace(parameterName)) throw new ArgumentNullException("parameterName");

            string stringValue = value as string;
            if (!string.IsNullOrWhiteSpace(stringValue)
                && length > 0
                && stringValue.Length > length) return false;

            return true;
        }

        internal T GetParameterValue<T>(string parameterName)
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

        internal void Add(string key, IDbDataParameter parameter)
        {
            this.Parameters.Add(key, parameter);
        }

        internal IDbDataParameter this[string key]
        {
            get { return this.Parameters[key]; }
        }

        internal bool ContainsKey(string key)
        {
            return this.Parameters.ContainsKey(key);
        }

        IEnumerator<IDbDataParameter> IEnumerable<IDbDataParameter>.GetEnumerator()
        {
            return (IEnumerator<IDbDataParameter>)this.GetEnumerator();
        }
    }
}
