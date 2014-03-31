using RabbitDB.ChangeTracker;
using RabbitDB.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.Entity.ChangeRecorder
{
    internal class NotifiedChangeRecorder : BaseChangeRecorder, IChangeRecorder, IDisposable
    {
        private ITracker _tracker;

        private Dictionary<string, object> NotifiedValues { get; set; }

        public NotifiedChangeRecorder(ITracker tracker, IValidEntityArgumentsReader validEntityArgumentsReader)
            : base(validEntityArgumentsReader)
        {
            _tracker = tracker;
            _tracker.IsDirtyChanged += UpdateOrCreateHashSet;
            this.NotifiedValues = new Dictionary<string, object>();
        }

        private void UpdateOrCreateHashSet(object sender, IsDiryChangedArgs args)
        {
            if (this.NotifiedValues.ContainsKey(args.PropertyName))
            {
                if (args.IsDirty)
                    this.NotifiedValues[args.PropertyName] = args.NewValue;
                else
                    this.NotifiedValues.Remove(args.PropertyName);
            }
            else
            {
                this.NotifiedValues.Add(args.PropertyName, args.NewValue);
            }
        }

        public override KeyValuePair<string, object>[] ComputeValuesToUpdate()
        {
            var validEntityValues = base.ValidArgumentReader.ReadValidEntityArguments();
            var valuesToUpdate = new Dictionary<string, object>();

            foreach (var kvp in this.NotifiedValues)
            {
                if (validEntityValues.Any(arg => arg.Key == kvp.Key))
                    valuesToUpdate.Add(kvp.Key, validEntityValues.FirstOrDefault(kvp1 => kvp1.Key == kvp.Key).Value);
            }

            return valuesToUpdate.ToArray();
        }

        private void Dispose(bool dispose)
        {
            if (dispose && base._disposed == false)
            {
                _tracker.Dispose();
                this.NotifiedValues.Clear();
                this.NotifiedValues = null;
                _tracker.IsDirtyChanged -= UpdateOrCreateHashSet;

                base._disposed = true;
            }
        }

        public override void Dispose()
        {
            Dispose(true);
        }
    }
}