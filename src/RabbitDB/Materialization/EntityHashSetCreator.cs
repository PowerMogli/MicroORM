// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityHashSetCreator.cs" company="">
//   
// </copyright>
// <summary>
//   The entity hash set creator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Collections.Generic;
using System.Linq;

using RabbitDB.Contracts.Materialization;
using RabbitDB.Reflection;

#endregion

namespace RabbitDB.Materialization
{
    /// <summary>
    ///     The entity hash set creator.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class EntityHashSetCreator<TEntity> : IEntityHashSetCreator
    {
        #region Fields

        /// <summary>
        ///     The _entity.
        /// </summary>
        private readonly TEntity _entity;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="EntityHashSetCreator{TEntity}" /> class.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        internal EntityHashSetCreator(TEntity entity)
        {
            _entity = entity;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The compute entity hash set.
        /// </summary>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        public Dictionary<string, int> ComputeEntityHashSet()
        {
            KeyValuePair<string, object>[] keyValuePairs = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { _entity });

            // if (keyValuePairs.Length >= 30)
            // return ComputeParallelEntityHashSet(keyValuePairs);
            return ComputeEntityHashSet(keyValuePairs);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The compute entity hash set.
        /// </summary>
        /// <param name="keyValuePairs">
        ///     The key value pairs.
        /// </param>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        private static Dictionary<string, int> ComputeEntityHashSet(
            IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            return keyValuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value != null
                ? kvp.Value.GetHashCode()
                : -1);
        }

        #endregion

        // Parallel.ForEach(keyValuePairs, (kvp, loopState, elementIndex) =>
        // var processedKeyValuePairs = new KeyValuePair<string, int>[keyValuePairs.Length];
        // {

        // internal static Dictionary<string, int> ComputeEntityHashSetInParallel(KeyValuePair<string, object>[] keyValuePairs)
        // {
        // processedKeyValuePairs[elementIndex] = new KeyValuePair<string, int>(kvp.Key, kvp.Value != null ? kvp.Value.GetHashCode() : -1);
        // });
        // return processedKeyValuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        // }
    }
}