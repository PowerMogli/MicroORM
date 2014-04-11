// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HashedChangeRecorder.cs" company="">
//   
// </copyright>
// <summary>
//   The hashed change recorder.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Entity.ChangeRecorder
{
    using System.Collections.Generic;
    using System.Linq;

    using RabbitDB.Materialization;
    using RabbitDB.Utils;

    /// <summary>
    /// The hashed change recorder.
    /// </summary>
    internal class HashedChangeRecorder : BaseChangeRecorder
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="HashedChangeRecorder"/> class.
        /// </summary>
        /// <param name="entityHashSetCreator">
        /// The entity hash set creator.
        /// </param>
        /// <param name="validEntityArgumentsReader">
        /// The valid entity arguments reader.
        /// </param>
        internal HashedChangeRecorder(
            IEntityHashSetCreator entityHashSetCreator, 
            IValidEntityArgumentsReader validEntityArgumentsReader)
            : base(validEntityArgumentsReader)
        {
            this.EntityHashSetCreator = entityHashSetCreator;
            this.ValueSnapshot = new Dictionary<string, int>();
            this.ChangesSnapshot = new Dictionary<string, int>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the changes snapshot.
        /// </summary>
        private Dictionary<string, int> ChangesSnapshot { get; set; }

        /// <summary>
        /// Gets or sets the entity hash set creator.
        /// </summary>
        private IEntityHashSetCreator EntityHashSetCreator { get; set; }

        /// <summary>
        /// Gets or sets the value snapshot.
        /// </summary>
        private Dictionary<string, int> ValueSnapshot { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The clear changes.
        /// </summary>
        public override void ClearChanges()
        {
            this.ChangesSnapshot.Clear();
        }

        /// <summary>
        /// The compute snapshot.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        public override void ComputeSnapshot<TEntity>(TEntity entity)
        {
            this.ValueSnapshot = EntityHashSetCreator.ComputeEntityHashSet();
        }

        /// <summary>
        /// The compute values to update.
        /// </summary>
        /// <returns>
        /// The <see cref="KeyValuePair"/>.
        /// </returns>
        public override KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            var entityHashSet = EntityHashSetCreator.ComputeEntityHashSet();
            var entityValues = ValidArgumentReader.ReadValidEntityArguments();

            var valuesToUpdate = new Dictionary<string, object>();
            foreach (var kvp in entityHashSet)
            {
                var oldHash = this.ValueSnapshot[kvp.Key];
                if (oldHash.Equals(kvp.Value))
                {
                    continue;
                }

                valuesToUpdate.Add(kvp.Key, entityValues.FirstOrDefault(kvp1 => kvp1.Key == kvp.Key).Value);
                this.ChangesSnapshot.Add(kvp.Key, kvp.Value);
            }

            return valuesToUpdate.ToArray();
        }

        /// <summary>
        /// The dispose.
        /// </summary>
        public override void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// The merge changes.
        /// </summary>
        public override void MergeChanges()
        {
            foreach (var change in this.ChangesSnapshot)
            {
                this.ValueSnapshot[change.Key] = change.Value;
            }

            ClearChanges();
        }

        #endregion

        #region Methods

        /// <summary>
        /// The dispose.
        /// </summary>
        /// <param name="dispose">
        /// The dispose.
        /// </param>
        private void Dispose(bool dispose)
        {
            if (!dispose || base.Disposed)
            {
                return;
            }

            this.ValidArgumentReader = null;
            this.EntityHashSetCreator = null;
            this.ValueSnapshot.Clear();
            this.ValueSnapshot = null;
            this.ChangesSnapshot.Clear();
            this.ChangesSnapshot = null;

            base.Disposed = true;
        }

        #endregion
    }
}