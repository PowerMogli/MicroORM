using RabbitDB.Mapping;
using System;

namespace RabbitDB.Crosscut.Mapping
{
    internal interface ITableInfo
    {
        Tuple<bool, string> GetIdentityType();
        PropertyInfoCollection Columns { get; }
    }
}