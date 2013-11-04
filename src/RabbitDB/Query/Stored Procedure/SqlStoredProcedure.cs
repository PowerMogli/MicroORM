using System.Data;
using System.Data.SqlClient;

namespace RabbitDB.Query.StoredProcedure
{
    public class SqlStoredProcedure : StoredProcedure
    {
        public SqlStoredProcedure(string storedProcedureName)
            : base(storedProcedureName) { }

        protected override bool AddParameter<T>(string parameterName, T value, DbType dbType, int length = -1)
        {
            return AddParameter<T>(parameterName, value, dbType, length, ParameterDirection.Input);
        }

        protected bool AddParameter<T>(string parameterName, T value, DbType dbType, int length = -1, ParameterDirection parameterDirection = default(ParameterDirection))
        {
            if (base.Parameters.AddParameter(parameterName, value, dbType, length) == false) return false;

            string prefix = "@";
            if (parameterName.StartsWith("@"))
                prefix = string.Empty;

            SqlParameter parameter = new SqlParameter(prefix + parameterName, value) { DbType = dbType };
            if (length > 0)
                parameter.Size = length;

            if (base.Parameters.ContainsKey(prefix + parameterName.ToLower()))
                base.Parameters[prefix + parameterName.ToLower()].Value = value;
            else
                base.Parameters.Add(prefix + parameterName.ToLower(), parameter);
            return true;
        }

        protected override T GetParameterValue<T>(string parameterName)
        {
            return this.Parameters.GetParameterValue<T>(parameterName);
        }
    }
}