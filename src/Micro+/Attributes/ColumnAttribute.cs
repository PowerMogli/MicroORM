using System;
using System.Data;

namespace MicroORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ColumnAttribute : NamedAttribute
    {
        //private bool _identifier;
        private int _size = -1;

        public ColumnAttribute()
        {
            this.IsNullable = true;
        }

        public ColumnAttribute(string columnName)
        {
            if (columnName == null)
                throw new ArgumentNullException("columnName");

            this.ColumnName = columnName;
            this.IsNullable = true;
        }

        public override string ColumnName { get; set; }

        public DbType DbType { get; set; }

        internal DbType? NullDbType { get { return new Nullable<DbType>(this.DbType); } }
        //public bool Identifier
        //{
        //    get { return _identifier; }
        //    set { _identifier = value; }
        //}

        public bool IsNullable { get; set; }

        public bool AutoNumber { get; set; }

        public int Size
        {
            get { return _size; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Value is below zero. Not allowed.");

                _size = value;
            }
        }
    }
}
