#region using directives

using System;
using System.Collections.Generic;

#endregion

namespace RabbitDB.Contracts.Entity
{
    internal interface IEntity : IDisposable
    {
        #region  Properties

        IEntityInfo EntityInfo { get; }

        bool IsForDeletion { get; }

        bool IsForInsert { get; }

        bool IsForUpdate { get; }

        #endregion

        #region Public Methods

        KeyValuePair<string, object>[] ComputeValuesToUpdate();

        void RaiseEntityDeleted();

        void RaiseEntityInserted();

        void RaiseEntityUpdated();

        #endregion
    }
}