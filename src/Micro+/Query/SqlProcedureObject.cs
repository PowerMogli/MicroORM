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
            if (base.AddParameter(parameterName, value, dbType, length) == false) return false;

            SqlParameter parameter = new SqlParameter(parameterName, value) { DbType = dbType };
            if (length > 0)
                parameter.Size = length;

            base.Parameters.Add(parameterName, parameter);
            return true;
        }
    }
}
