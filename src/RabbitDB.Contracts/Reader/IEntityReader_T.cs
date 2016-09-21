#region using directives

using System;
using System.Collections.Generic;
using System.Data;

#endregion

namespace RabbitDB.Contracts.Reader
{
    public interface IEntityReader<TEntity> : IDisposable
    {
        #region  Properties

        TEntity Current { get; }

        #endregion

        #region Public Methods

        bool Load(TEntity entity);

        IEnumerable<TEntity> Load(Func<IDataReader, IEnumerable<TEntity>> materializer);

        void Load(TEntity entity, Action<TEntity, IDataReader> materializer);

        bool Read();

        #endregion
    }
}