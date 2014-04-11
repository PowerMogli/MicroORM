// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEntityHashSetCreator.cs" company="">
//   
// </copyright>
// <summary>
//   The EntityHashSetCreator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace RabbitDB.Materialization
{
    using System.Collections.Generic;

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