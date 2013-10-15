using System;

namespace MicroORM.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ColumnAttribute : NamedAttribute
    {
        private bool _isNullable = true;
        //private bool _identifier;
        private bool _autoNumber;
        private int _size = -1;

        public ColumnAttribute()
        {
        }

        public ColumnAttribute(string columndName)
        {
            if (columndName == null)
                throw new ArgumentNullException("columnName");

            this.ColumnName = columndName;
        }

        public override string ColumnName { get; set; }

        //public bool Identifier
        //{
        //    get { return _identifier; }
        //    set { _identifier = value; }
        //}

        public bool IsNullable
        {
            get { return _isNullable; }
            set { _isNullable = value; }
        }

        public bool AutoNumber
        {
            get { return _autoNumber; }
            set { _autoNumber = value; }
        }

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
