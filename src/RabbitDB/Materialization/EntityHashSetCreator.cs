// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityHashSetCreator.cs" company="">
//   
// </copyright>
// <summary>
//   The entity hash set creator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Materialization
{
    using System.Collections.Generic;
    using System.Linq;

    using RabbitDB.Reflection;

    /// <summary>
    /// The entity hash set creator.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class EntityHashSetCreator<TEntity> : IEntityHashSetCreator
    {
        #region Fields

        /// <summary>
        /// The _entity.
        /// </summary>
        private readonly TEntity _entity;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityHashSetCreator{TEntity}"/> class.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal EntityHashSetCreator(TEntity entity)
        {
            this._entity = entity;
        }

        #endregion

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
        public Dictionary<string, int> ComputeEntityHashSet()
        {
            var keyValuePairs = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { this._entity });

            // if (keyValuePairs.Length >= 30)
            // return ComputeParallelEntityHashSet(keyValuePairs);
            return ComputeEntityHashSet(keyValuePairs);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The compute entity hash set.
        /// </summary>
        /// <param name="keyValuePairs">
        /// The key value pairs.
        /// </param>
        /// <returns>
        /// The <see>
        ///         <cref>Dictionary</cref>
        ///     </see>
        ///     .
        /// </returns>
        private static Dictionary<string, int> ComputeEntityHashSet(
            IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            return keyValuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value != null ? kvp.Value.GetHashCode() : -1);
        }

        #endregion

        // internal static Dictionary<string, int> ComputeEntityHashSetInParallel(KeyValuePair<string, object>[] keyValuePairs)
        // {
        // var processedKeyValuePairs = new KeyValuePair<string, int>[keyValuePairs.Length];

        // Parallel.ForEach(keyValuePairs, (kvp, loopState, elementIndex) =>
        // {
        // processedKeyValuePairs[elementIndex] = new KeyValuePair<string, int>(kvp.Key, kvp.Value != null ? kvp.Value.GetHashCode() : -1);
        // });
        // return processedKeyValuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        // }
    }
}