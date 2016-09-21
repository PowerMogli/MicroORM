#region using directives

using System.Collections;
using System.Collections.Generic;

#endregion

namespace RabbitDB.Contracts.Query
{
    public interface IQueryParameterCollection<T> : IQueryParameterCollection,
                                                    IList<T>,
                                                    IReadOnlyList<T>
    {
    }

    public interface IQueryParameterCollection : IList
    {
        #region Public Methods

        void AddRange(object[] arguments);

        #endregion
    }
}