// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Entity.cs" company="">
//   
// </copyright>
// <summary>
//   The entity.
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
    ///     The entity.
    /// </summary>
    public abstract class Entity : IEntity
    {
        #region  Properties

        /// <summary>
        ///     The entity deleted.
        /// </summary>
        public event EventHandler EntityDeleted = delegate { };

        /// <summary>
        ///     The entity inserted.
        /// </summary>
        public event EventHandler EntityInserted = delegate { };

        /// <summary>
        ///     The entity updated.
        /// </summary>
        public event EventHandler EntityUpdated = delegate { };

        /// <summary>
        ///     Gets or sets the entity info.
        /// </summary>
        public IEntityInfo EntityInfo { get; set; }

        /// <summary>
        ///     The is for deletion.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool IsForDeletion => MarkedForDeletion && EntityInfo.EntityState != EntityState.Deleted;

        /// <summary>
        ///     The is for insert.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool IsForInsert => EntityInfo.EntityState == EntityState.Deleted || EntityInfo.EntityState == EntityState.None;

        /// <summary>
        ///     The is for update.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool IsForUpdate => EntityInfo.EntityState != EntityState.Deleted;

        /// <summary>
        ///     Gets the change tracer option.
        /// </summary>
        internal virtual ChangeRecorderOption ChangeTracerOption => ChangeRecorderOption.Hashed;

        /// <summary>
        ///     The has changes.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        internal bool HasChanges => MarkedForDeletion || EntityInfo.HasChanges();

        /// <summary>
        ///     The is loaded.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        internal bool IsLoaded => EntityInfo != null && EntityInfo.EntityState == EntityState.Loaded;

        /// <summary>
        ///     Gets or sets a value indicating whether marked for deletion.
        /// </summary>
        internal bool MarkedForDeletion { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The compute values to update.
        /// </summary>
        /// <returns>
        ///     The <see cref="KeyValuePair" />.
        /// </returns>
        public KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            return EntityInfo.ComputeValuesToUpdate();
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public void Dispose()
        {
            EntityInfo.Dispose();
        }

        /// <summary>
        ///     The raise entity deleted.
        /// </summary>
        public void RaiseEntityDeleted()
        {
            EntityDeleted(this, EventArgs.Empty);
        }

        /// <summary>
        ///     The raise entity inserted.
        /// </summary>
        public void RaiseEntityInserted()
        {
            EntityInserted(this, EventArgs.Empty);
        }

        /// <summary>
        ///     The raise entity updated.
        /// </summary>
        public void RaiseEntityUpdated()
        {
            EntityUpdated(this, EventArgs.Empty);
        }

        #endregion
    }
}