// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="">
//   
// </copyright>
// <summary>
//   The entity.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Entity
{
    using System;
    using System.Collections.Generic;

    using RabbitDB.Entity.ChangeRecorder;

    /// <summary>
    /// The entity.
    /// </summary>
    public abstract class Entity : IDisposable
    {
        #region Public Events

        /// <summary>
        /// The entity deleted.
        /// </summary>
        public event EventHandler EntityDeleted = delegate { };

        /// <summary>
        /// The entity inserted.
        /// </summary>
        public event EventHandler EntityInserted = delegate { };

        /// <summary>
        /// The entity updated.
        /// </summary>
        public event EventHandler EntityUpdated = delegate { };

        #endregion

        #region Properties

        /// <summary>
        /// Gets the change tracer option.
        /// </summary>
        internal virtual ChangeRecorderOption ChangeTracerOption
        {
            get
            {
                return ChangeRecorderOption.Hashed;
            }
        }

        /// <summary>
        /// Gets or sets the entity info.
        /// </summary>
        internal EntityInfo EntityInfo { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether marked for deletion.
        /// </summary>
        internal bool MarkedForDeletion { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            this.EntityInfo.Dispose();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The compute values to update.
        /// </summary>
        /// <returns>
        /// The <see cref="KeyValuePair"/>.
        /// </returns>
        internal KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            return this.EntityInfo.ComputeValuesToUpdate();
        }

        /// <summary>
        /// The has changes.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool HasChanges()
        {
            return MarkedForDeletion || this.EntityInfo.HasChanges();
        }

        /// <summary>
        /// The is for deletion.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool IsForDeletion()
        {
            return this.MarkedForDeletion && this.EntityInfo.EntityState != EntityState.Deleted;
        }

        /// <summary>
        /// The is for insert.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool IsForInsert()
        {
            return this.EntityInfo.EntityState == EntityState.Deleted || this.EntityInfo.EntityState == EntityState.None;
        }

        /// <summary>
        /// The is for update.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool IsForUpdate()
        {
            return this.EntityInfo.EntityState != EntityState.Deleted;
        }

        /// <summary>
        /// The is loaded.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        internal bool IsLoaded()
        {
            return this.EntityInfo != null && this.EntityInfo.EntityState == EntityState.Loaded;
        }

        /// <summary>
        /// The raise entity deleted.
        /// </summary>
        internal void RaiseEntityDeleted()
        {
            this.EntityDeleted(this, EventArgs.Empty);
        }

        /// <summary>
        /// The raise entity inserted.
        /// </summary>
        internal void RaiseEntityInserted()
        {
            this.EntityInserted(this, EventArgs.Empty);
        }

        /// <summary>
        /// The raise entity updated.
        /// </summary>
        internal void RaiseEntityUpdated()
        {
            this.EntityUpdated(this, EventArgs.Empty);
        }

        #endregion
    }
}