using RabbitDB.ChangeTracker;
using System.Collections.Generic;
using System.Linq;

namespace RabbitDB.Entity
{
    internal sealed class NotifiedEntityInfo : EntityInfo
    {
        private Dictionary<string, object> NotifiedValues { get; set; }

        internal NotifiedEntityInfo()
        {
            NotifiedValues = new Dictionary<string, object>();
        }
        internal void UpdateOrCreateHashSet(IsDiryChangedArgs args)
        {
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
        
        internal override KeyValuePair<string, object>[] ComputeValuesToUpdate<TEntity>(TEntity entity, IEnumerable<KeyValuePair<string, object>> entityValues)
        {
            return NotifiedValues.ToArray();
        }
    }
}