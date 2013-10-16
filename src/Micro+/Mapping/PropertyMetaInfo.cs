using System;
using System.Reflection;
using MicroORM.Attributes;
using MicroORM.Reflection;
using System.Data;

namespace MicroORM.Mapping
{
    internal sealed class PropertyMetaInfo : MetaInfo
    {
        private PropertyInfo _propertyInfo;

        public PropertyMetaInfo(PropertyInfo propertyInfo, Type propertyType, NamedAttribute columnAttribute)
            : this(propertyInfo, propertyType, 0, columnAttribute) { }

        public PropertyMetaInfo(PropertyInfo propertyInfo, Type propertyType, DbType dbType, NamedAttribute columnAttribute)
            : base(propertyType, dbType, columnAttribute)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if ((!propertyInfo.CanWrite || !propertyInfo.CanRead))
            {
                throw new TableInfoException(
                    string.Format("Cannot create mapping for {0}.{1} because it's not possible to write and read.",
                    propertyInfo.ReflectedType.FullName, propertyInfo.Name));
            }
            _propertyInfo = propertyInfo;
        }

        /// <summary>
        /// Returns the name of the element of the persistent object type that is mapped to the field in the storage.
        /// </summary>
        public override string Name { get { return _propertyInfo.Name; } }

        /// <summary>
        /// Returns true if the MemberInfo is write-able.
        /// </summary>
        public override bool CanWrite
        {
            get { return _propertyInfo.CanWrite; }
        }

        public override object GetValue(object obj)
        {
            return _propertyInfo.GetValueFast(obj);
        }

        public override void SetValue(object obj, object value)
        {
            if (this.IsNullable && this.PropertyType.BaseType == typeof(Enum))
            {
                Type type = typeof(Nullable<>).MakeGenericType(this.PropertyType);
                if (value == null)
                    value = Activator.CreateInstance(type);
                else
                    value = Activator.CreateInstance(type, Enum.ToObject(this.PropertyType, value));
            }

            _propertyInfo.SetValue(obj, value, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            //_propertyInfo.SetValueFast(obj, value);
        }
    }
}
