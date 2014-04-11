using System;
using System.Collections.Generic;

namespace RabbitDB.Crosscut.Entity
{
    public interface IEntity
    {
        event EventHandler EntityDeleted;
        event EventHandler EntityInserted;
        event EventHandler EntityUpdated;
        KeyValuePair<string, object>[] ComputeValuesToUpdate();
    }
}