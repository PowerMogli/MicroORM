#region using directives

using System.Collections.Generic;

#endregion

namespace RabbitDB.Contracts.Schema
{
    public interface IDbTable
    {
        #region  Properties

        List<IDbColumn> DbColumns { get; }

        #endregion
    }
}