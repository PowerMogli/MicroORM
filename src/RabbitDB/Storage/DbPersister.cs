// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbPersister.cs" company="">
//   
// </copyright>
// <summary>
//   The db persister.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    using RabbitDB.Mapping;
    using RabbitDB.Query;
    using RabbitDB.SqlBuilder;
    using RabbitDB.Utils;

    /// <summary>
    /// The db persister.
    /// </summary>
    internal class DbPersister : IDbPersister
    {
        #region Fields

        /// <summary>
        /// The _db command executor.
        /// </summary>
        private readonly ICommandExecutor _dbCommandExecutor;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DbPersister"/> class.
        /// </summary>
        /// <param name="dbCommandExecutor">
        /// The db command executor.
        /// </param>
        internal DbPersister(ICommandExecutor dbCommandExecutor)
        {
            _dbCommandExecutor = dbCommandExecutor;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public void Delete<TEntity>(TEntity entity)
        {
            var tableInfo = TableInfo<TEntity>.GetTableInfo;
            var deleteStatement = SqlBuilder<TEntity>.DeleteStatement;
            var arguments = QueryParameterCollection.Create<TEntity>(tableInfo.GetPrimaryKeyValues(entity));

            _dbCommandExecutor.ExecuteCommand(new SqlQuery(deleteStatement, arguments));
        }

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public void Insert<TEntity>(TEntity entity)
        {
            var tableInfo = TableInfo<TEntity>.GetTableInfo;
            var insertStatement = SqlBuilder<TEntity>.InsertStatement;
            var arguments =
                QueryParameterCollection.Create<TEntity>(
                    new EntityArgumentsReader().GetEntityArguments(entity, tableInfo));

            var insertId = _dbCommandExecutor.ExecuteScalar<object>(new SqlQuery(insertStatement, arguments));
            tableInfo.SetAutoNumber(entity, insertId);
        }

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public void Update<TEntity>(IQuery query)
        {
            _dbCommandExecutor.ExecuteCommand(query);
        }

        #endregion
    }
}