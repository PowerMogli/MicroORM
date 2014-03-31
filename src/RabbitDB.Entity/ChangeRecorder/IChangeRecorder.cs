using System;
using System.Collections.Generic;

namespace RabbitDB.Entity.ChangeRecorder
{
    internal interface IChangeRecorder : IDisposable
    {
        KeyValuePair<string, object>[] ComputeValuesToUpdate();
        void MergeChanges();
        void ClearChanges();
        void ComputeSnapshot<TEntity>(TEntity entity);
    }
}