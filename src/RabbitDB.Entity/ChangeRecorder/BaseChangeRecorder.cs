using RabbitDB.Utils;
using System;
using System.Collections.Generic;

namespace RabbitDB.Entity.ChangeRecorder
{
    internal abstract class BaseChangeRecorder : IChangeRecorder, IDisposable
    {
        protected bool _disposed;

        public BaseChangeRecorder(IValidEntityArgumentsReader validEntityArgumentsReader)
        {
            this.ValidArgumentReader = validEntityArgumentsReader;
        }

        protected IValidEntityArgumentsReader ValidArgumentReader { get; set; }

        public abstract KeyValuePair<string, object>[] ComputeValuesToUpdate();

        public virtual void MergeChanges() { /* Do Nothing */ }

        public virtual void ClearChanges() { /* Do Nothing */ }

        public virtual void ComputeSnapshot<TEntity>(TEntity entity) { /* Do Nothing */ }

        public abstract void Dispose();
    }
}