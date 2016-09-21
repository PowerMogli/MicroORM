// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseDbSession.cs" company="">
//   
// </copyright>
// <summary>
//   The base db session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Data;

using RabbitDB.Contracts;
using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.Reader;
using RabbitDB.Contracts.Session;
using RabbitDB.Contracts.Storage;
using RabbitDB.Schema;
using RabbitDB.SqlDialect;
using RabbitDB.Storage;

#endregion

namespace RabbitDB.Session
{
    /// <summary>
    ///     The base db session.
    /// </summary>
    public abstract class BaseDbSession : IBaseDbSession
    {
        #region Fields

        /// <summary>
        ///     The _db persister.
        /// </summary>
        private IDbPersister _dbPersister;

        /// <summary>
        ///     The _db engine.
        /// </summary>
        protected DbEngine DbEngine;

        /// <summary>
        ///     The _disposed.
        /// </summary>
        protected bool Disposed;

        /// <summary>
        ///     The _sql dialect.
        /// </summary>
        internal SqlDialect.SqlDialect SqlDialect;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDbSession" /> class.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        /// <param name="dbEngine">
        ///     The db engine.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        protected BaseDbSession(string connectionString, DbEngine dbEngine)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "The connectionString cannot be null or empty!");
            }

            DbEngine = dbEngine;

            Initialize(connectionString);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDbSession" /> class.
        /// </summary>
        /// <param name="assemblyType">
        ///     The assembly type.
        /// </param>
        protected BaseDbSession(Type assemblyType)
        {
            string connectionString = Registrar<string>.GetFor(assemblyType);

            DbEngine = Registrar<DbEngine>.GetFor(assemblyType);

            Initialize(connectionString);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseDbSession" /> class.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        protected BaseDbSession(string connectionString)
            : this(connectionString, DbEngine.SqlServer)
        {
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the db persister.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        internal IDbPersister DbPersister
        {
            get
            {
                if (SqlDialect == null)
                {
                    throw new InvalidOperationException("SqlDialect is not initialized");
                }

                return _dbPersister ?? (_dbPersister = new DbPersister(SqlDialect));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The begin transaction.
        /// </summary>
        /// <param name="isolationLevel">
        ///     The isolation level.
        /// </param>
        /// <returns>
        ///     The <see cref="IDbTransaction" />.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// </exception>
        public IDbTransaction BeginTransaction(IsolationLevel? isolationLevel = null)
        {
            ITransactionalDbProvider transactionalProvider = SqlDialect.DbProvider as ITransactionalDbProvider;
            if (transactionalProvider == null)
            {
                throw new NotSupportedException(
                    $@"This type of DbEngine ('{DbEngine}') does not implement the interface ITransactionalDbProvider
                                  and so does not support transactional behavior.");
            }

            return isolationLevel.HasValue
                ? transactionalProvider.BeginTransaction(isolationLevel.Value)
                : transactionalProvider.BeginTransaction();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        internal void Dispose()
        {
            // ReSharper disable once RedundantCheckBeforeAssignment
            if (_dbPersister != null)
            {
                _dbPersister = null;
            }

            SqlDialect.Dispose();

            DbSchemaAllocator.SchemaReader.Dispose();

            Disposed = true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        void IDisposable.Dispose()
        {
            if (Disposed)
            {
                return;
            }

            Dispose();
        }

        /// <summary>
        ///     The get entity reader.
        /// </summary>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>EntityReader</cref>
        ///     </see>
        ///     .
        /// </returns>
        IEntityReader<TEntity> IBaseDbSession.GetEntityReader<TEntity>(IQuery query)
        {
            return SqlDialect.ExecuteReader<TEntity>(query);
        }

        /// <summary>
        ///     The get entity set.
        /// </summary>
        /// <param name="query">
        ///     The query.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>EntitySet</cref>
        ///     </see>
        ///     .
        /// </returns>
        IEntitySet<TEntity> IBaseDbSession.GetEntitySet<TEntity>(IQuery query)
        {
            EntitySet<TEntity> objectSet = new EntitySet<TEntity>();

            return objectSet.Load(this, query);
        }

        /// <summary>
        ///     The initialize.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        private void Initialize(string connectionString)
        {
            SqlDialect = SqlDialectFactory.Create(DbEngine, connectionString);

            DbProviderAccessor.SqlDialect = SqlDialect;
        }

        #endregion
    }
}