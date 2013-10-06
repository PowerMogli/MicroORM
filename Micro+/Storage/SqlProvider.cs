using System;
using System.Collections.Concurrent;
using System.Data;
using MicroORM.Base.Query;

namespace MicroORM.Base.Storage
{
    internal class SqlDbProvider : DbProvider
    {
        private const string _providerName = "System.Data.SqlClient";

        public override string ParameterPrefix { get { return "@"; } }
        public override string ProviderName { get { return _providerName; } }

        internal SqlDbProvider(string connectionString)
            : base(connectionString) { }

        public override void ExecuteCommand(IQuery query)
        {
            base.CreateConnection();
            IDbCommand command = query.Compile(this);
            command.Transaction = DatabaseTransaction.Transaction;

            command.ExecuteNonQuery();
        }

        public override ObjectReader<T> ExecuteReader<T>(IQuery query)
        {
            base.CreateConnection();

            IDbCommand command = query.Compile(this);
            command.Transaction = DatabaseTransaction.Transaction;

            return null;
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
