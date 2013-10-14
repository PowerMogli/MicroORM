using System;
using System.Data;
using MicroORM.Query;

namespace MicroORM.Storage
{
    internal class SqlDbProvider : DbProvider, ITransactionalDbProvider, IDbProvider
    {
        private const string _providerName = "System.Data.SqlClient";

        public override string ParameterPrefix { get { return "@"; } }
        public override string ProviderName { get { return _providerName; } }

        internal SqlDbProvider(string connectionString)
            : base(connectionString) { }

        public IDbTransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (base._dbTransaction != null) return base._dbTransaction;
            if (base._dbConnection != null) return base._dbTransaction = base._dbConnection.BeginTransaction(isolationLevel);

            base.CreateConnection();
            return base._dbTransaction = base._dbConnection.BeginTransaction(isolationLevel);
        }

        public IDbTransaction BeginTransaction()
        {
            if (base._dbTransaction != null) return base._dbTransaction;
            if (base._dbConnection != null) return base._dbTransaction = base._dbConnection.BeginTransaction();

            base.CreateConnection();
            return base._dbTransaction = base._dbConnection.BeginTransaction();
        }

        public override string EscapeName(string value)
        {
            return "[" + value + "]";
        }

        public override object ExecuteInsert(IQuery query)
        {
            ((SqlQuery)query).SqlStatement += ";Select SCOPE_IDENTITY() as id";
            return base.ExecuteScalar<decimal>(query);
        }

        public override void SetupParameter(IDbDataParameter parameter, string name, object value)
        {
            base.SetupParameter(parameter, name, value);
            if (value == null) return;

            DealWithSpecialParameterValues(parameter, value);
        }

        private static void DealWithSpecialParameterValues(IDbDataParameter parameter, object value)
        {
            Type valueType = value.GetType();

            if (valueType == typeof(string))
            {
                parameter.Size = Math.Max(((string)value).Length + 1, 4000);
            }
            else if (valueType.Name == "SqlGeography")
            {
                dynamic param = parameter;
                param.UdtTypeName = "geography";
            }
            else if (valueType.Name == "SqlGeometry") //SqlGeography is a CLR Type
            {
                dynamic param = parameter;
                param.UdtTypeName = "geometry";
            }
        }
    }
}
