using System;
using MicroORM.Attributes;
using System.Data;

namespace MicroORM.Mapping
{
    internal interface IPropertyInfo
    {
        NamedAttribute ColumnAttribute { get; }

        DbType? DbType { get; }

        object GetValue(object obj);

        void SetValue(object obj, object value);

        bool IsNullable { get; }

        bool CanWrite { get; }

        Type PropertyType { get; }

        string Name { get; }
    }
}
