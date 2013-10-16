using System;
using System.Reflection;
using MicroORM.Attributes;
using System.Data;

namespace MicroORM.Mapping
{
    internal abstract class MetaInfo : IPropertyInfo
    {
        private bool _isNullable;
        private Type _propertyType;

        internal MetaInfo(Type propertyType, NamedAttribute columnAttribute)
        {
            if (propertyType == null)
                throw new ArgumentNullException("propertyType");

            this.PropertyType = propertyType;
            this.ColumnAttribute = columnAttribute;
        }

        internal MetaInfo(Type propertyType, DbType dbType, NamedAttribute columnAttribute)
            : this(propertyType, columnAttribute)
        {
            this.DbType = dbType;

            _isNullable = (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>));
            if (_isNullable)
            {
                // Set the generic type of the nullable as type of the object.
                _propertyType = propertyType.GetGenericArguments()[0];
            }
        }

        public NamedAttribute ColumnAttribute { get; private set; }

        public DbType? DbType { get; private set; }

        public bool IsNullable
        {
            get { return _isNullable; }
        }

        public Type PropertyType { get; private set; }

        /// <summary>
        /// Returns the name of the element of the persistent object type that is mapped to the field in the storage.
        /// </summary>
        public abstract string Name { get; }

        public abstract bool CanWrite { get; }

        /// <summary>
        /// Sets new value for the element of the persistent object type that is mapped to the storage.
        /// </summary>
        /// <param name="obj">The persistent object that's member's value is updated.</param>
        /// <param name="value">The new value.</param>
        public abstract void SetValue(object obj, object value);

        /// <summary>
        /// Returns the value of the element of the persistent object type that is mapped to a field in the storage.
        /// </summary>
        /// <param name="obj">The persistent object that's value is returned.</param>
        /// <returns>Value of the persistent object's type element that is mapped to a field in the storage.</returns>
        public abstract object GetValue(object obj);
    }
}
