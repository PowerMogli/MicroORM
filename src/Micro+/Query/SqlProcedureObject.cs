﻿using System;
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
    }
}
