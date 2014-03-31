using RabbitDB.Materialization;
using RabbitDB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.Entity.ChangeRecorder
{
    internal class HashedChangeRecorder : BaseChangeRecorder, IChangeRecorder, IDisposable
    {
        private IEntityHashSetCreator EntityHashSetCreator { get; set; }
        private Dictionary<string, int> ValueSnapshot { get; set; }
        private Dictionary<string, int> ChangesSnapshot { get; set; }

        internal HashedChangeRecorder(IEntityHashSetCreator entityHashSetCreator, IValidEntityArgumentsReader validEntityArgumentsReader)
            : base(validEntityArgumentsReader)
        {
            this.EntityHashSetCreator = entityHashSetCreator;
            this.ValueSnapshot = new Dictionary<string, int>();
            this.ChangesSnapshot = new Dictionary<string, int>();
        }

        public override KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            var entityHashSet = EntityHashSetCreator.ComputeEntityHashSet();
            var entityValues = ValidArgumentReader.ReadValidEntityArguments();

            var valuesToUpdate = new Dictionary<string, object>();
            foreach (var kvp in entityHashSet)
            {
                var oldHash = this.ValueSnapshot[kvp.Key];
                if (oldHash.Equals(kvp.Value) == false)
                {
                    valuesToUpdate.Add(kvp.Key, entityValues.FirstOrDefault(kvp1 => kvp1.Key == kvp.Key).Value);
                    this.ChangesSnapshot.Add(kvp.Key, kvp.Value);
                }
            }

            return valuesToUpdate.ToArray();
        }

        public override void ClearChanges()
        {
            this.ChangesSnapshot.Clear();
        }

        public override void MergeChanges()
        {
            foreach (var change in this.ChangesSnapshot)
            {
                this.ValueSnapshot[change.Key] = change.Value;
            }
            ClearChanges();
        }

        public override void ComputeSnapshot<TEntity>(TEntity entity)
        {
            this.ValueSnapshot = EntityHashSetCreator.ComputeEntityHashSet();
        }

        private void Dispose(bool dispose)
        {
            if (dispose && base._disposed == false)
            {
                this.ValidArgumentReader = null;
                this.EntityHashSetCreator = null;
                this.ValueSnapshot.Clear();
                this.ValueSnapshot = null;
                this.ChangesSnapshot.Clear();
                this.ChangesSnapshot = null;

                base._disposed = true;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
        }
    }
}