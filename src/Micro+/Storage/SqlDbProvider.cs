using System;
using System.Collections.Concurrent;
using System.Data;
using MicroORM.Base;
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

        static ConcurrentDictionary<Type,Tuple<bool,bool,bool>> _meta=new ConcurrentDictionary<Type, Tuple<bool, bool, bool>>();
        public override void SetupParameter(IDbDataParameter parameter, string name, object value)
        {
            base.SetupParameter(parameter, name, value);
            if (value == null) return;

            Type valueType = value.GetType();

            Tuple<bool, bool, bool> meta = null;

            if (!_meta.TryGetValue(valueType, out meta))
            {
                meta = new Tuple<bool, bool, bool>(valueType == typeof(string), valueType.Name == "SqlGeography", valueType.Name == "SqlGeometry");
                _meta.TryAdd(valueType, meta);
            }

            if (meta.Item1)
            {
                parameter.Size = Math.Max(((string)value).Length + 1, 4000);
            }
            else if (meta.Item2) //SqlGeography is a CLR Type
            {
                dynamic p = parameter;
                p.UdtTypeName = "geography";
            }
            else if (meta.Item3) //SqlGeometry is a CLR Type
            {
                dynamic p = parameter;
                p.UdtTypeName = "geometry";
            }
        }
    }
}
