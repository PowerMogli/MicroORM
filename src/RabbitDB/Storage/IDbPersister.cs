// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDbPersister.cs" company="">
//   
// </copyright>
// <summary>
//   The DbPersister interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Storage
{
    using RabbitDB.Query;

    /// <summary>
    /// The DbPersister interface.
    /// </summary>
    internal interface IDbPersister
    {
        #region Public Methods and Operators

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        void Delete<TEntity>(TEntity entity);

        /// <summary>
        /// The insert.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        void Insert<TEntity>(TEntity entity);

        /// <summary>
        /// The update.
        /// </summary>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        void Update<TEntity>(IQuery query);

        #endregion
    }
}