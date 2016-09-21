// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheItem.cs" company="">
//   
// </copyright>
// <summary>
//   The cache item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;

using RabbitDB.Entity;
using RabbitDB.Entity.Entity;

#endregion

namespace RabbitDB.Caching
{
    /// <summary>
    ///     The cache item.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class CacheItem<TEntity>
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="CacheItem{TEntity}" /> class.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <param name="entityInfo">
        ///     The entity info.
        /// </param>
        internal CacheItem(TEntity entity, EntityInfo entityInfo)
        {
            EntityInfo = entityInfo;
            Reference = new WeakReference(entity);
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets or sets the entity info.
        /// </summary>
        internal EntityInfo EntityInfo { get; set; }

        /// <summary>
        ///     Gets a value indicating whether is alive.
        /// </summary>
        internal bool IsAlive => Reference.IsAlive;

        /// <summary>
        ///     Gets the target.
        /// </summary>
        internal TEntity Target => (TEntity)Reference.Target;

        /// <summary>
        ///     Gets or sets the reference.
        /// </summary>
        private WeakReference Reference { get; }

        #endregion
    }
}