using System.Collections;
using System.Collections.Generic;
using System.Data;

namespace MicroORM.Query
{
    public abstract class ProcedureObject : IEnumerable
    {
        private string _storedProcedureName = string.Empty;

        internal ProcedureObject(string storedProcedureName)
        {
            _storedProcedureName = storedProcedureName;
        }

        internal string StoredProcedureName { get { return _storedProcedureName; } }
        private Dictionary<string, IDataParameter> _parameters = new Dictionary<string, IDataParameter>();

        protected Dictionary<string, IDataParameter> Parameters { get { return _parameters; } }

        protected abstract bool AddParameter<T>(string parameterName, T value, DbType dbType, int length);
        protected abstract T GetParameterValue<T>(string parameterName);

        public IEnumerator GetEnumerator()
        {
            return this.Parameters.Values.GetEnumerator();
        }
    }
}
