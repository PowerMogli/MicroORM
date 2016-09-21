// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseChangeRecorder.cs" company="">
//   
// </copyright>
// <summary>
//   The base change recorder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Collections.Generic;

using RabbitDB.Utils;

#endregion

namespace RabbitDB.Entity.ChangeRecorder
{
    /// <summary>
    ///     The base change recorder.
    /// </summary>
    internal abstract class BaseChangeRecorder : IChangeRecorder
    {
        #region Fields

        /// <summary>
        ///     The _disposed.
        /// </summary>
        protected bool Disposed;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseChangeRecorder" /> class.
        /// </summary>
        /// <param name="validEntityArgumentsReader">
        ///     The valid entity arguments reader.
        /// </param>
        protected BaseChangeRecorder(IValidEntityArgumentsReader validEntityArgumentsReader)
        {
            ValidArgumentReader = validEntityArgumentsReader;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BaseChangeRecorder" /> class.
        /// </summary>
        protected BaseChangeRecorder()
        {
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets or sets the valid argument reader.
        /// </summary>
        protected IValidEntityArgumentsReader ValidArgumentReader { get; set; }

        #endregion

        #region Public Methods

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
        public abstract KeyValuePair<string, object>[] ComputeValuesToUpdate();

        /// <summary>
        ///     The dispose.
        /// </summary>
        public abstract void Dispose();

        #endregion

        #region Public Methods

        /// <summary>
        ///     The clear changes.
        /// </summary>
        public virtual void ClearChanges()
        {
            /* Do Nothing */
        }

        /// <summary>
        ///     The compute snapshot.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public virtual void ComputeSnapshot<TEntity>(TEntity entity)
        {
            /* Do Nothing */
        }

        /// <summary>
        ///     The merge changes.
        /// </summary>
        public virtual void MergeChanges()
        {
            /* Do Nothing */
        }

        #endregion
    }
}