// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashedChangeRecorder.cs" company="">
//   
// </copyright>
// <summary>
//   The hashed change recorder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Collections.Generic;
using System.Linq;

using RabbitDB.Contracts.Materialization;
using RabbitDB.Utils;

#endregion

namespace RabbitDB.Entity.ChangeRecorder
{
    /// <summary>
    ///     The hashed change recorder.
    /// </summary>
    internal class HashedChangeRecorder : BaseChangeRecorder
    {
        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="HashedChangeRecorder" /> class.
        /// </summary>
        /// <param name="entityHashSetCreator">
        ///     The entity hash set creator.
        /// </param>
        /// <param name="validEntityArgumentsReader">
        ///     The valid entity arguments reader.
        /// </param>
        internal HashedChangeRecorder(
            IEntityHashSetCreator entityHashSetCreator,
            IValidEntityArgumentsReader validEntityArgumentsReader)
            : base(validEntityArgumentsReader)
        {
            EntityHashSetCreator = entityHashSetCreator;
            ValueSnapshot = new Dictionary<string, int>();
            ChangesSnapshot = new Dictionary<string, int>();
        }

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets or sets the changes snapshot.
        /// </summary>
        private Dictionary<string, int> ChangesSnapshot { get; set; }

        /// <summary>
        ///     Gets or sets the entity hash set creator.
        /// </summary>
        private IEntityHashSetCreator EntityHashSetCreator { get; set; }

        /// <summary>
        ///     Gets or sets the value snapshot.
        /// </summary>
        private Dictionary<string, int> ValueSnapshot { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The clear changes.
        /// </summary>
        public override void ClearChanges()
        {
            ChangesSnapshot.Clear();
        }

        /// <summary>
        ///     The compute snapshot.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public override void ComputeSnapshot<TEntity>(TEntity entity)
        {
            ValueSnapshot = EntityHashSetCreator.ComputeEntityHashSet();
        }

        /// <summary>
        ///     The compute values to update.
        /// </summary>
        /// <returns>
        ///     The <see cref="KeyValuePair" />.
        /// </returns>
        public override KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            Dictionary<string, int> entityHashSet = EntityHashSetCreator.ComputeEntityHashSet();
            IEnumerable<KeyValuePair<string, object>> entityValues = ValidArgumentReader.ReadValidEntityArguments();

            Dictionary<string, object> valuesToUpdate = new Dictionary<string, object>();
            foreach (KeyValuePair<string, int> kvp in entityHashSet)
            {
                int oldHash = ValueSnapshot[kvp.Key];
                if (oldHash.Equals(kvp.Value))
                {
                    continue;
                }

                valuesToUpdate.Add(kvp.Key, entityValues.FirstOrDefault(kvp1 => kvp1.Key == kvp.Key)
                                                        .Value);
                ChangesSnapshot.Add(kvp.Key, kvp.Value);
            }

            return valuesToUpdate.ToArray();
        }

        /// <summary>
        ///     The dispose.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///     The merge changes.
        /// </summary>
        public override void MergeChanges()
        {
            foreach (KeyValuePair<string, int> change in ChangesSnapshot)
            {
                ValueSnapshot[change.Key] = change.Value;
            }

            ClearChanges();
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
            if (!dispose || Disposed)
            {
                return;
            }

            ValidArgumentReader = null;
            EntityHashSetCreator = null;
            ValueSnapshot.Clear();
            ValueSnapshot = null;
            ChangesSnapshot.Clear();
            ChangesSnapshot = null;

            Disposed = true;
        }

        #endregion
    }
}