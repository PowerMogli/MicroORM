// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IChangeRecorder.cs" company="">
//   
// </copyright>
// <summary>
//   The ChangeRecorder interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RabbitDB.Entity.ChangeRecorder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The ChangeRecorder interface.
    /// </summary>
    internal interface IChangeRecorder : IDisposable
    {
        #region Public Methods and Operators

        /// <summary>
        /// The clear changes.
        /// </summary>
        void ClearChanges();

        /// <summary>
        /// The compute snapshot.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        void ComputeSnapshot<TEntity>(TEntity entity);

        /// <summary>
        /// The compute values to update.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>KeyValuePair</cref>
        ///     </see>
        ///     .
        /// </returns>
        KeyValuePair<string, object>[] ComputeValuesToUpdate();

        /// <summary>
        /// The merge changes.
        /// </summary>
        void MergeChanges();

        #endregion
    }
}