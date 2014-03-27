using RabbitDB.ChangeTracker;
using RabbitDB.Utils;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.Entity
{
    internal sealed class NotifiedEntityInfo : EntityInfo
    {
        private ITracker _tracker;
        private Dictionary<string, object> NotifiedValues { get; set; }

        internal NotifiedEntityInfo(ITracker tracker, IValidEntityArgumentsReader validEntityArgumentsReader)
            : base()
        {
            _tracker = tracker;
            _tracker.IsDirtyChanged += UpdateOrCreateHashSet;
            this.ValidArgumentReader = validEntityArgumentsReader;
            this.NotifiedValues = new Dictionary<string, object>();
        }

        internal void UpdateOrCreateHashSet(object sender, IsDiryChangedArgs args)
        {
            if (this.EntityState != EntityState.Loaded) return;

            if (this.NotifiedValues.ContainsKey(args.PropertyName))
            {
                this.NotifiedValues[args.PropertyName] = args.NewValue;
            }
            else
            {
                this.NotifiedValues.Add(args.PropertyName, args.NewValue);
            }
        }

        internal override void ComputeSnapshot<TEntity>(TEntity entity) { /* Do nothing */ }

        internal override void MergeChanges() { /* Do nothing */ }

        internal override KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            var validEntityValues = this.ValidArgumentReader.ReadValidEntityArguments();
            var valuesToUpdate = new Dictionary<string, object>();

            foreach (var kvp in this.NotifiedValues)
            {
                if (validEntityValues.Any(arg => arg.Key == kvp.Key))
                    valuesToUpdate.Add(kvp.Key, validEntityValues.FirstOrDefault(kvp1 => kvp1.Key == kvp.Key).Value);
            }

            return valuesToUpdate.ToArray();
        }

        protected override void Dispose(bool dispose)
        {
            if (base._disposed == false)
            {
                base.Dispose(dispose);

                _tracker.Dispose();
                this.NotifiedValues.Clear();
                this.NotifiedValues = null;
            }
        }
    }
}