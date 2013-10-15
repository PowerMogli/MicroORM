using System;

namespace MicroORM.Attributes
{
    public abstract class NamedAttribute : Attribute
    {
        public abstract string ColumnName { get; set; }
    }
}
