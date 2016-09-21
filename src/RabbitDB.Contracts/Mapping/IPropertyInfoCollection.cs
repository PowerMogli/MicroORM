#region using directives

using System.Collections.Generic;

using RabbitDB.Contracts.Schema;
using RabbitDB.Contracts.SqlDialect;

#endregion

namespace RabbitDB.Contracts.Mapping
{
    internal interface IPropertyInfoCollection : IEnumerable<IPropertyInfo>
    {
        #region  Properties

        int Count { get; }

        IPropertyInfo this[int index] { get; }

        #endregion

        #region Public Methods

        void Add(IPropertyInfo propertyInfo);

        bool Contains(string columnName);

        void Remove(string name);

        IEnumerable<string> SelectValidColumnNames(IDbTable table, ISqlCharacters sqlCharacters);

        IEnumerable<string> SelectValidNonAutoNumberColumnNames(ISqlCharacters sqlCharacters);

        IEnumerable<string> SelectValidNonAutoNumberPrefixedColumnNames();

        #endregion
    }
}