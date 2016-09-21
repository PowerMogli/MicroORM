// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The entity info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections.Generic;

using RabbitDB.Contracts.Entity;
using RabbitDB.Entity.ChangeRecorder;

#endregion

namespace RabbitDB.Entity.Entity
{
    /// <summary>
    ///     The entity info.
    /// </summary>
    internal class EntityInfo : IEntityInfo,
                                IChangeRecorder
    {
        #region Fields

        /// <summary>
        ///     The _change tracer.
        /// </summary>
        private readonly IChangeRecorder _changeTracer;

        /// <summary>
        ///     The _disposed.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityInfo" /> class.
        /// </summary>
        /// <param name="changeTracer">
        ///     The change tracer.
        /// </param>
        internal EntityInfo(IChangeRecorder changeTracer)
            : this()
        {
            _changeTracer = changeTracer;
        }

        /// <summary>
        ///     Prevents a default instance of the <see cref="EntityInfo" /> class from being created.
        /// </summary>
        private EntityInfo()
        {
            EntityState = EntityState.None;
            LastCallTime = DateTime.Now;
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets or sets the entity state.
        /// </summary>
        public EntityState EntityState { get; set; }

        /// <summary>
        ///     Gets a value indicating whether can be removed.
        /// </summary>
        internal bool CanBeRemoved => DateTime.Now.Subtract(LastCallTime) > TimeSpan.FromMinutes(2);

        /// <summary>
        ///     Gets or sets the last call time.
        /// </summary>
        internal DateTime LastCallTime { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The clear changes.
        /// </summary>
        public void ClearChanges()
        {
            _changeTracer.ClearChanges();
        }

        /// <summary>
        ///     The compute snapshot.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public void ComputeSnapshot<TEntity>(TEntity entity)
        {
            _changeTracer.ComputeSnapshot(entity);
        }

        /// <summary>
        ///     The compute values to update.
        /// </summary>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>KeyValuePair</cref>
        ///     </see>
        ///     .
        /// </returns>
        public KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            return _changeTracer.ComputeValuesToUpdate();
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     The has changes.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool HasChanges()
        {
            KeyValuePair<string, object>[] valuesToUpdate = ComputeValuesToUpdate();

            return valuesToUpdate.Length > 0
                   || EntityState == EntityState.Deleted
                   || EntityState == EntityState.None;
        }

        /// <summary>
        ///     The merge changes.
        /// </summary>
        public void MergeChanges()
        {
            _changeTracer.MergeChanges();
        }

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The update entity info last call time.
        /// </summary>
        internal void UpdateLastCallTime()
        {
            LastCallTime = DateTime.Now;
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The dispose.
        /// </summary>
        /// <param name="dispose">
        ///     The dispose.
        /// </param>
        private void Dispose(bool dispose)
        {
            if (!dispose || _disposed)
            {
                return;
            }

            _changeTracer.Dispose();

            _disposed = true;
        }

        #endregion
    }
}