using System;

namespace MicroORM.Base
{
    [Serializable]
    public enum EntityState
    {
        /// <summary>
        /// Persistent objects with this state aren't loaded, saved or deleted. They have been recently created in memory.
        /// </summary>
        None,
        /// <summary>
        /// Persistent objects with this state have been loaded from the storage.
        /// </summary>
        Loaded,
        /// <summary>
        /// Persistent objects with this state have been inserted into the storage. The framework is doing an
        /// update if the persistent is saved again.
        /// </summary>
        Inserted,
        /// <summary>
        /// Persistent objects with this state have been updated in the storage.
        /// </summary>
        Updated,
        /// <summary>
        /// Persistent objects with this state are no longer available in the storage. They have
        /// been deleted. The framework is doing an insert if they are saved the next time.
        /// </summary>
        Deleted
    }
}
