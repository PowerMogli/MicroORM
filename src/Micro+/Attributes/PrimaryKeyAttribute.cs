using System;

namespace MicroORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : NamedAttribute
    {
        public override string ColumnName { get; set; }

        public PrimaryKeyAttribute() { }

        public PrimaryKeyAttribute(string columnName)
        {
            if (columnName == null)
                throw new ArgumentNullException("columnName");

            this.ColumnName = columnName;
        }
    }
}
