#region using directives

using System;

#endregion

namespace RabbitDB.Contracts.Mapping
{
    internal interface ITableInfo
    {
        #region  Properties

        IPropertyInfoCollection Columns { get; }

        #endregion

        #region Public Methods

        Tuple<bool, string> GetIdentityType();

        #endregion
    }
}