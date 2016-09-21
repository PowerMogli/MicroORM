// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbSession.cs" company="">
//   
// </copyright>
// <summary>
//   The db session.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Linq.Expressions;

using RabbitDB.Contracts.Reader;
using RabbitDB.Contracts.Session;
using RabbitDB.Query;
using RabbitDB.Query.Generic;
using RabbitDB.Storage;

#endregion

namespace RabbitDB.Session
{
    /// <summary>
    ///     The db session.
    /// </summary>
    public sealed class DbSession : ReadOnlySession,
                                    IDbSession
    {
        #region Fields

        /// <summary>
        ///     The _db entity persister.
        /// </summary>
        private DbEntityPersister _dbEntityPersister;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbSession" /> class.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        /// <param name="dbEngine">
        ///     The db engine.
        /// </param>
        public DbSession(string connectionString, DbEngine dbEngine)
            : base(connectionString, dbEngine)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbSession" /> class.
        /// </summary>
        /// <param name="assemblyType">
        ///     The assembly type.
        /// </param>
        public DbSession(Type assemblyType)
            : base(assemblyType)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbSession" /> class.
        /// </summary>
        /// <param name="connectionString">
        ///     The connection string.
        /// </param>
        public DbSession(string connectionString)
            : this(connectionString, DbEngine.SqlServer)
        {
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="DbSession" /> class.
        /// </summary>
        ~DbSession()
        {
            if (Disposed == false)
            {
                Dispose();
            }
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the configuration.
        /// </summary>
        public static Configuration Configuration => Configuration.Instance;

        /// <summary>
        ///     Gets the db entity persister.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// </exception>
        internal DbEntityPersister DbEntityPersister
        {
            get
            {
                if (SqlDialect == null)
                {
                    throw new InvalidOperationException("SqlDialect is not initialized");
                }

                return _dbEntityPersister ?? (_dbEntityPersister = new DbEntityPersister(DbPersister));
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The delete.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public void Delete<TEntity>(TEntity entity)
        {
            DbPersister.Delete(entity);
        }

        /// <summary>
        ///     The execute command.
        /// </summary>
        /// <param name="sql">
        ///     The sql.
        /// </param>
        /// <param name="arguments">
        ///     The arguments.
        /// </param>
        public void ExecuteCommand(string sql, params object[] arguments)
        {
            SqlDialect.ExecuteCommand(new SqlQuery(sql, QueryParameterCollection.Create(arguments)));
        }

        /// <summary>
        ///     The insert.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public void Insert<TEntity>(TEntity entity)
        {
            DbPersister.Insert(entity);
        }

        /// <summary>
        ///     The update.
        /// </summary>
        /// <param name="criteria">
        ///     The criteria.
        /// </param>
        /// <param name="setArguments">
        ///     The set arguments.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public void Update<TEntity>(Expression<Func<TEntity, bool>> criteria, params object[] setArguments)
        {
            DbPersister.Update<TEntity>(new UpdateExpressionQuery<TEntity>(criteria, setArguments));
        }

        /// <summary>
        ///     The update.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public void Update<TEntity>(TEntity entity)
        {
            DbPersister.Update<TEntity>(new UpdateQuery<TEntity>(entity));
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The load.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <exception cref="Exception">
        /// </exception>
        void IDbSession.Load<TEntity>(TEntity entity)
        {
            IEntityReader<TEntity> objectReader = SqlDialect.ExecuteReader<TEntity>(new EntityQuery<TEntity>(entity));

            if (objectReader.Load(entity) == false)
            {
                throw new Exception("Loading data was not successfull!");
            }
        }

        /// <summary>
        ///     The persist changes.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        bool IDbSession.PersistChanges<TEntity>(TEntity entity)
        {
            return DbEntityPersister.PersistChanges(entity);
        }

        #endregion
    }
}