#region using directives

using System;
using System.Collections.Generic;

#endregion

namespace RabbitDB.Contracts.Entity
{
    public interface IEntityInfo : IDisposable
    {
        #region  Properties

        EntityState EntityState { get; set; }

        #endregion

        #region Public Methods

        void ClearChanges();

        void ComputeSnapshot<TEntity>(TEntity entity);

        KeyValuePair<string, object>[] ComputeValuesToUpdate();

        bool HasChanges();

        void MergeChanges();

        #endregion
    }
}