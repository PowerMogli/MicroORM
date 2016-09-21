// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityHashSetCreator.cs" company="">
//   
// </copyright>
// <summary>
//   The EntityHashSetCreator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace RabbitDB.Contracts.Materialization
{
    /// <summary>
    /// The EntityHashSetCreator interface.
    /// </summary>
    internal interface IEntityHashSetCreator
    {
        #region Public Methods and Operators

        /// <summary>
        /// The compute entity hash set.
        /// </summary>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        Dictionary<string, int> ComputeEntityHashSet();

        #endregion
    }
}