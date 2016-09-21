// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbEntityPersister.cs" company="">
//   
// </copyright>
// <summary>
//   The db entity persister.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;

using RabbitDB.Contracts.Entity;
using RabbitDB.Contracts.Query;
using RabbitDB.Contracts.Storage;
using RabbitDB.Entity;
using RabbitDB.Query;

#endregion

namespace RabbitDB.Storage
{
    /// <summary>
    ///     The db entity persister.
    /// </summary>
    internal class DbEntityPersister
    {
        #region Fields

        /// <summary>
        ///     The _db persister.
        /// </summary>
        private readonly IDbPersister _dbPersister;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbEntityPersister" /> class.
        /// </summary>
        /// <param name="dbPersister">
        ///     The db persister.
        /// </param>
        internal DbEntityPersister(IDbPersister dbPersister)
        {
            _dbPersister = dbPersister;
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     This method is ONLY for POCOs
        ///     who inherit from Entity.Entity
        /// </summary>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <param name="entity">
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        internal bool PersistChanges<TEntity>(TEntity entity)
            where TEntity : IEntity
        {
            return (entity.IsForDeletion && Delete(entity))
                   || (entity.IsForInsert && Insert(entity))
                   || (entity.IsForUpdate && Update(entity));
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The delete.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool Delete<TEntity>(TEntity entity)
            where TEntity : IEntity
        {
            _dbPersister.Delete(entity);

            entity.RaiseEntityDeleted();

            entity.EntityInfo.EntityState = EntityState.Deleted;

            return true;
        }

        /// <summary>
        ///     The insert.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool Insert<TEntity>(TEntity entity)
            where TEntity : IEntity
        {
            _dbPersister.Insert(entity);

            entity.RaiseEntityInserted();

            entity.EntityInfo.EntityState = EntityState.Inserted;

            return true;
        }

        /// <summary>
        ///     The update.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool Update<TEntity>(TEntity entity)
            where TEntity : IEntity
        {
            Tuple<bool, string, QueryParameterCollection> tuple = entity.PrepareForUpdate();

            return tuple.Item1 && Update(entity, new SqlQuery(tuple.Item2, tuple.Item3));
        }

        /// <summary>
        ///     The update.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <param name="sqlQuery">
        ///     The sql query.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        private bool Update<TEntity>(TEntity entity, IQuery sqlQuery)
            where TEntity : IEntity
        {
            _dbPersister.Update<TEntity>(sqlQuery);

            entity.RaiseEntityUpdated();

            entity.EntityInfo.EntityState = EntityState.Updated;

            return true;
        }

        #endregion
    }
}