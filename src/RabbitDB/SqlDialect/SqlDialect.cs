using RabbitDB.Expressions;
using RabbitDB.Mapping;
using RabbitDB.Query;
using RabbitDB.Reader;
using RabbitDB.Storage;
using System;
using System.Data;

namespace RabbitDB.SqlDialect
{
    internal abstract class SqlDialect : ICommandExecutor, INullValueResolver, IDisposable
    {
        internal SqlCharacters SqlCharacters { get; set; }
        internal IDbCommandExecutor DbCommandExecutor { get; private set; }
        internal IDbProvider DbProvider { get; private set; }

        // for unit tests
        internal SqlDialect(SqlCharacters sqlCharacters, IDbProvider dbProvider, IDbCommandExecutor dbCommandExecutor)
            : this(sqlCharacters, dbProvider)
        {
            this.DbCommandExecutor = dbCommandExecutor;
            this.DbCommandExecutor.NullValueResolver = this;
        }

        internal SqlDialect(SqlCharacters sqlCharacters, IDbProvider dbProvider)
        {
            this.DbProvider = dbProvider;
            this.DbCommandExecutor = new DbCommandExecutor(this.DbProvider, this);
            this.SqlCharacters = sqlCharacters;
        }

        private IDbCommand CompileCommand(IQuery query)
        {
            return query.Compile(this);
        }

        private IDbCommand PrepareCommand(IQuery query)
        {
            this.DbProvider.CreateConnection();
            var dbCommand = CompileCommand(query);
            this.DbProvider.SetupCommand(dbCommand);

            return dbCommand;
        }

        public virtual object ResolveNullValue(object value, Type type)
        {
            if (value == null
                || (value is DBNull
                && !type.IsSubclassOf(typeof(ValueType)))) return null;

            Type originalType = type.UnderlyingSystemType;

            if (originalType == typeof(short)) return (short)0;
            else if (originalType == typeof(int)) return (int)0;
            else if (originalType == typeof(long)) return (long)0;
            else if (originalType == typeof(byte)) return (byte)0;
            else if (originalType == typeof(Single)) return (Single)0;
            else if (originalType == typeof(decimal)) return (decimal)0;
            else if (originalType == typeof(double)) return (double)0;
            else if (originalType == typeof(string)) return string.Empty;
            else if (originalType == typeof(bool)) return false;
            else if (originalType == typeof(DateTime)) return new DateTime();
            else if (originalType == typeof(byte[]) || originalType == typeof(object)) return new byte[] { };

            throw new InvalidTypeException("Unsupported type encountered while converting from DBNull.");
        }

        internal abstract string ResolveScopeIdentity(TableInfo tableInfo);

        internal abstract string ScopeIdentity { get; }

        internal abstract IDbProviderExpressionBuildHelper BuilderHelper { get; }

        public void ExecuteCommand(IQuery query)
        {
            var dbCommand = PrepareCommand(query);
            this.DbCommandExecutor.ExecuteCommand(dbCommand);
        }

        public IDataReader ExecuteReader(IQuery query)
        {
            var dbCommand = PrepareCommand(query);
            return this.DbCommandExecutor.ExecuteReader(dbCommand);
        }

        public EntityReader<T> ExecuteReader<T>(IQuery query)
        {
            var dbCommand = PrepareCommand(query);
            return this.DbCommandExecutor.ExecuteReader<T>(dbCommand);
        }

        public T ExecuteScalar<T>(IQuery query)
        {
            var dbCommand = PrepareCommand(query);
            return this.DbCommandExecutor.ExecuteScalar<T>(dbCommand);
        }

        public void Dispose()
        {
            this.DbProvider.Dispose();
        }
    }
}