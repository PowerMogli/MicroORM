// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CacheItem.cs" company="">
//   
// </copyright>
// <summary>
//   The cache item.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Caching
{
    using System;

    using RabbitDB.Entity;

    /// <summary>
    /// The cache item.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class CacheItem<TEntity>
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheItem{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="entityInfo">
        /// The entity info.
        /// </param>
        internal CacheItem(TEntity entity, EntityInfo entityInfo)
        {
            this.EntityInfo = entityInfo;
            this.Reference = new WeakReference(entity);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the entity info.
        /// </summary>
        internal EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// Gets a value indicating whether is alive.
        /// </summary>
        internal bool IsAlive
        {
            get
            {
                return this.Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the target.
        /// </summary>
        internal TEntity Target
        {
            get
            {
                return (TEntity)this.Reference.Target;
            }
        }

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        private WeakReference Reference { get; set; }

        #endregion
    }
}