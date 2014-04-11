// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseDbSession.cs" company="">
//   
// </copyright>
// <summary>
//   The base db session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Session
{
    using System;
    using System.Data;

    using RabbitDB.Query;
    using RabbitDB.Reader;
    using RabbitDB.Schema;
    using RabbitDB.SqlDialect;
    using RabbitDB.Storage;

    /// <summary>
    /// The base db session.
    /// </summary>
    public abstract class BaseDbSession : IBaseDbSession
    {
        #region Fields

        /// <summary>
        /// The _sql dialect.
        /// </summary>
        internal SqlDialect SqlDialect = null;

        /// <summary>
        /// The _db engine.
        /// </summary>
        protected DbEngine DbEngine;

        /// <summary>
        /// The _disposed.
        /// </summary>
        protected bool Disposed;

        /// <summary>
        /// The _db persister.
        /// </summary>
        private IDbPersister _dbPersister;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbSession"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        /// <param name="dbEngine">
        /// The db engine.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        protected BaseDbSession(string connectionString, DbEngine dbEngine)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException("connectionString", "The connectionString cannot be null or empty!");
            }

            this.DbEngine = dbEngine;
            Initialize(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbSession"/> class.
        /// </summary>
        /// <param name="assemblyType">
        /// The assembly type.
        /// </param>
        protected BaseDbSession(Type assemblyType)
        {
            string connectionString = Registrar<string>.GetFor(assemblyType);
            this.DbEngine = Registrar<DbEngine>.GetFor(assemblyType);
            Initialize(connectionString);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseDbSession"/> class.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        protected BaseDbSession(string connectionString)
            : this(connectionString, DbEngine.SqlServer)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the db persister.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        internal IDbPersister DbPersister
        {
            get
            {
                if (this.SqlDialect == null)
                {
                    throw new InvalidOperationException("SqlDialect is not initialized");
                }

                return _dbPersister ?? (_dbPersister = new DbPersister(this.SqlDialect));
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The begin transaction.
        /// </summary>
        /// <param name="isolationLevel">
        /// The isolation level.
        /// </param>
        /// <returns>
        /// The <see cref="IDbTransaction"/>.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            var transactionalProvider = this.SqlDialect.DbProvider as ITransactionalDbProvider;
            if (transactionalProvider == null)
            {
                throw new NotSupportedException(
                    string.Format(
                        @"This type of DbEngine ('{0}') does not implement the interface ITransactionalDbProvider
                                  and so does not support transactional behavior.", 
                        this.DbEngine));
            }

            return isolationLevel.HasValue
                       ? transactionalProvider.BeginTransaction(isolationLevel.Value)
                       : transactionalProvider.BeginTransaction();
        }

        #endregion

        #region Explicit Interface Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (this.Disposed)
            {
                return;
            }

            Dispose();
        }

        /// <summary>
        /// The get entity reader.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        EntityReader<TEntity> IBaseDbSession.GetEntityReader<TEntity>(IQuery query)
        {
            return this.SqlDialect.ExecuteReader<TEntity>(query);
        }

        /// <summary>
        /// The get entity set.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        EntitySet<TEntity> IBaseDbSession.GetEntitySet<TEntity>(IQuery query)
        {
            var objectSet = new EntitySet<TEntity>();

            return objectSet.Load(this, query);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        internal void Dispose()
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (_dbPersister != null)
            {
                _dbPersister = null;
            }

            this.SqlDialect.Dispose();
            DbSchemaAllocator.SchemaReader.Dispose();
            this.Disposed = true;
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="connectionString">
        /// The connection string.
        /// </param>
        private void Initialize(string connectionString)
        {
            this.SqlDialect = SqlDialectFactory.Create(this.DbEngine, connectionString);
            DbProviderAccessor.SqlDialect = this.SqlDialect;
        }

        #endregion
    }
}