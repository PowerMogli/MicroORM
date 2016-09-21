// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDialect.cs" company="">
//   
// </copyright>
// <summary>
//   The sql dialect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Data;

using RabbitDB.Contracts.Expressions;
using RabbitDB.Contracts.Mapping;
using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.Reader;
using RabbitDB.Contracts.SqlDialect;
using RabbitDB.Contracts.Storage;
using RabbitDB.Storage;

#endregion

namespace RabbitDB.SqlDialect
{
    /// <summary>
    ///     The sql dialect.
    /// </summary>
    internal abstract class SqlDialect : ISqlDialect,
                                         ICommandExecutor,
                                         INullValueResolver,
                                         IDisposable
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlDialect" /> class.
        /// </summary>
        /// <param name="sqlCharacters">
        ///     The sql characters.
        /// </param>
        /// <param name="dbProvider">
        ///     The db provider.
        /// </param>
        /// <param name="dbCommandExecutor">
        ///     The db command executor.
        /// </param>
        protected SqlDialect(ISqlCharacters sqlCharacters, IDbProvider dbProvider, IDbCommandExecutor dbCommandExecutor)
            : this(sqlCharacters, dbProvider)
        {
            DbCommandExecutor = dbCommandExecutor;
            DbCommandExecutor.NullValueResolver = this;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SqlDialect" /> class.
        /// </summary>
        /// <param name="sqlCharacters">
        ///     The sql characters.
        /// </param>
        /// <param name="dbProvider">
        ///     The db provider.
        /// </param>
        protected SqlDialect(ISqlCharacters sqlCharacters, IDbProvider dbProvider)
        {
            DbProvider = dbProvider;
            DbCommandExecutor = new DbCommandExecutor(DbProvider, this);
            SqlCharacters = sqlCharacters;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the builder helper.
        /// </summary>
        public abstract IDbProviderExpressionBuildHelper BuilderHelper { get; }

        /// <summary>
        ///     Gets the db provider.
        /// </summary>
        public IDbProvider DbProvider { get; }

        /// <summary>
        ///     Gets or sets the sql characters.
        /// </summary>
        public ISqlCharacters SqlCharacters { get; set; }

        /// <summary>
        ///     Gets the db command executor.
        /// </summary>
        internal IDbCommandExecutor DbCommandExecutor { get; }

        /// <summary>
        ///     Gets the scope identity.
        /// </summary>
        internal abstract string ScopeIdentity { get; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The resolve scope identity.
        /// </summary>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public abstract string ResolveScopeIdentity(ITableInfo tableInfo);

        #endregion

        #region Public Methods

        /// <summary>
        ///     The resolve null value.
        /// </summary>
        /// <param name="value">
        ///     The value.
        /// </param>
        /// <param name="type">
        ///     The type.
        /// </param>
        /// <returns>
        ///     The <see cref="object" />.
        /// </returns>
        /// <exception cref="InvalidTypeException">
        /// </exception>
        public virtual object ResolveNullValue(object value, Type type)
        {
            if (value == null || (value is DBNull && !type.IsSubclassOf(typeof(ValueType))))
            {
                return null;
            }

            Type originalType = type.UnderlyingSystemType;

            if (originalType == typeof(short))
            {
                return (short)0;
            }

            if (originalType == typeof(int))
            {
                return 0;
            }

            if (originalType == typeof(long))
            {
                return (long)0;
            }

            if (originalType == typeof(byte))
            {
                return (byte)0;
            }

            if (originalType == typeof(float))
            {
                return (float)0;
            }

            if (originalType == typeof(decimal))
            {
                return (decimal)0;
            }

            if (originalType == typeof(double))
            {
                return (double)0;
            }

            if (originalType == typeof(string))
            {
                return string.Empty;
            }

            if (originalType == typeof(bool))
            {
                return false;
            }

            if (originalType == typeof(DateTime))
            {
                return new DateTime();
            }

            if (originalType == typeof(byte[]) || originalType == typeof(object))
            {
                return new byte[] { };
            }

            throw new InvalidTypeException("Unsupported type encountered while converting from DBNull.");
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            DbProvider.Dispose();
        }

        /// <summary>
        ///     The execute command.
        /// </summary>
        /// <param name="query">
        ///     The query.
        /// </param>
        public void ExecuteCommand(IQuery query)
        {
            IDbCommand dbCommand = DbProvider.PrepareCommand(query, this);

            DbCommandExecutor.ExecuteCommand(dbCommand);
        }

        /// <summary>
        ///     The execute reader.
        /// </summary>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <returns>
        ///     The <see cref="IDataReader" />.
        /// </returns>
        public IDataReader ExecuteReader(IQuery query)
        {
            IDbCommand dbCommand = DbProvider.PrepareCommand(query, this);

            return DbCommandExecutor.ExecuteReader(dbCommand);
        }

        /// <summary>
        ///     The execute reader.
        /// </summary>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        public IEntityReader<T> ExecuteReader<T>(IQuery query)
        {
            IDbCommand dbCommand = DbProvider.PrepareCommand(query, this);

            return DbCommandExecutor.ExecuteReader<T>(dbCommand);
        }

        /// <summary>
        ///     The execute scalar.
        /// </summary>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T" />.
        /// </returns>
        public T ExecuteScalar<T>(IQuery query)
        {
            IDbCommand dbCommand = DbProvider.PrepareCommand(query, this);

            return DbCommandExecutor.ExecuteScalar<T>(dbCommand);
        }

        #endregion

        // for unit tests
    }
}