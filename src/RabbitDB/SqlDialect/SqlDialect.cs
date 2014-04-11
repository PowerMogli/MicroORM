// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SqlDialect.cs" company="">
//   
// </copyright>
// <summary>
//   The sql dialect.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.SqlDialect
{
    using System;
    using System.Data;

    using RabbitDB.Expressions;
    using RabbitDB.Mapping;
    using RabbitDB.Query;
    using RabbitDB.Reader;
    using RabbitDB.Storage;

    /// <summary>
    /// The sql dialect.
    /// </summary>
    internal abstract class SqlDialect : ICommandExecutor, INullValueResolver, IDisposable
    {
        // for unit tests
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDialect"/> class.
        /// </summary>
        /// <param name="sqlCharacters">
        /// The sql characters.
        /// </param>
        /// <param name="dbProvider">
        /// The db provider.
        /// </param>
        /// <param name="dbCommandExecutor">
        /// The db command executor.
        /// </param>
        internal SqlDialect(SqlCharacters sqlCharacters, IDbProvider dbProvider, IDbCommandExecutor dbCommandExecutor)
            : this(sqlCharacters, dbProvider)
        {
            this.DbCommandExecutor = dbCommandExecutor;
            this.DbCommandExecutor.NullValueResolver = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDialect"/> class.
        /// </summary>
        /// <param name="sqlCharacters">
        /// The sql characters.
        /// </param>
        /// <param name="dbProvider">
        /// The db provider.
        /// </param>
        internal SqlDialect(SqlCharacters sqlCharacters, IDbProvider dbProvider)
        {
            this.DbProvider = dbProvider;
            this.DbCommandExecutor = new DbCommandExecutor(this.DbProvider, this);
            this.SqlCharacters = sqlCharacters;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the builder helper.
        /// </summary>
        internal abstract IDbProviderExpressionBuildHelper BuilderHelper { get; }

        /// <summary>
        /// Gets the db command executor.
        /// </summary>
        internal IDbCommandExecutor DbCommandExecutor { get; private set; }

        /// <summary>
        /// Gets the db provider.
        /// </summary>
        internal IDbProvider DbProvider { get; private set; }

        /// <summary>
        /// Gets the scope identity.
        /// </summary>
        internal abstract string ScopeIdentity { get; }

        /// <summary>
        /// Gets or sets the sql characters.
        /// </summary>
        internal SqlCharacters SqlCharacters { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.DbProvider.Dispose();
        }

        /// <summary>
        /// The execute command.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        public void ExecuteCommand(IQuery query)
        {
            var dbCommand = this.DbProvider.PrepareCommand(query, this);
            this.DbCommandExecutor.ExecuteCommand(dbCommand);
        }

        /// <summary>
        /// The execute reader.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <returns>
        /// The <see cref="IDataReader"/>.
        /// </returns>
        public IDataReader ExecuteReader(IQuery query)
        {
            var dbCommand = this.DbProvider.PrepareCommand(query, this);
            return this.DbCommandExecutor.ExecuteReader(dbCommand);
        }

        /// <summary>
        /// The execute reader.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        public EntityReader<T> ExecuteReader<T>(IQuery query)
        {
            var dbCommand = this.DbProvider.PrepareCommand(query, this);
            return this.DbCommandExecutor.ExecuteReader<T>(dbCommand);
        }

        /// <summary>
        /// The execute scalar.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="T">
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        public T ExecuteScalar<T>(IQuery query)
        {
            var dbCommand = this.DbProvider.PrepareCommand(query, this);
            return this.DbCommandExecutor.ExecuteScalar<T>(dbCommand);
        }

        /// <summary>
        /// The resolve null value.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        /// <exception cref="InvalidTypeException">
        /// </exception>
        public virtual object ResolveNullValue(object value, Type type)
        {
            if (value == null || (value is DBNull && !type.IsSubclassOf(typeof(ValueType))))
            {
                return null;
            }

            var originalType = type.UnderlyingSystemType;

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

            if (originalType == typeof(Single))
            {
                return (Single)0;
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

        #endregion

        #region Methods

        /// <summary>
        /// The resolve scope identity.
        /// </summary>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        internal abstract string ResolveScopeIdentity(TableInfo tableInfo);

        #endregion
    }
}