using System;
using System.Reflection;

namespace MicroORM.Mapping
{
    /// <summary>
    /// Class represents a meta element that is used to create the mapping between the persistent object and
    /// the field in the table.
    /// </summary>
    /// <remarks>
    /// This class represents a meta element that is used to create the mapping between a property
    /// of the persistent object and a field in the storage table.
    /// </remarks>
    internal sealed class PropertyMetaInfo : MetaInfo
    {
        private PropertyInfo _propertyInfo;

        /// <summary>
        /// Creates a new instance of the <see cref="PropertyMetaInfo">PropertyMetaInfo Class</see>.
        /// </summary>
        /// <param name="propertyInfo">The PropertyInfo object that represents the property in the persistent object's type.</param>
        /// <param name="memberType">The type of the MemberInfo object.</param>
        /// <param name="attribute">The <see cref="FieldAttribute">FieldAttribute</see> that contains the mapping information.</param>
        public PropertyMetaInfo(PropertyInfo propertyInfo, Type memberType, FieldAttribute attribute)
            : base(memberType, propertyInfo, attribute)
        {
            if (propertyInfo == null)
                throw new ArgumentNullException("propertyInfo");

            if ((!propertyInfo.CanWrite || !propertyInfo.CanRead))
            {
                throw new TypeMappingException(
                    string.Format("Cannot create mapping for {0}.{1} because it's not possible to write and read.",
                    propertyInfo.ReflectedType.FullName, propertyInfo.Name));
            }
            _propertyInfo = propertyInfo;
        }

        /// <summary>
        /// Returns the name of the element of the persistent object type that is mapped to the field in the storage.
        /// </summary>
        public override string Name
        {
            get { return _propertyInfo.Name; }
        }

        /// <summary>
        /// Returns the value of the element of the persistent object type that is mapped to a field in the storage.
        /// </summary>
        /// <param name="obj">The persistent object that's value is returned.</param>
        /// <returns>Value of the persistent object's type element that is mapped to a field in the storage.</returns>
        public override object GetValue(object obj)
        {
            return _propertyInfo.GetValue(obj, null);
        }

        /// <summary>
        /// Sets new value for the element of the persistent object type that is mapped to the storage.
        /// </summary>
        /// <param name="obj">The persistent object that's member's value is updated.</param>
        /// <param name="value">The new value.</param>
        public override void SetValue(object obj, object value)
        {
            // Check if the value is a nullable and if an enum.
            // Enums need special processing if nullable: that's because they come in as int and .NET seems not to be able to convert
            // it back to an enum + nullable (the combination kills it).
            if (this.IsNullable && this.MemberType.BaseType == typeof(Enum))
            {
                // Get the nullable type and create an instance of it.
                Type type = typeof(Nullable<>).MakeGenericType(this.MemberType);
                if (value == null)
                    value = Activator.CreateInstance(type);
                else
                    value = Activator.CreateInstance(type, Enum.ToObject(this.MemberType, value));
            }

            _propertyInfo.SetValue(obj, value, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
        }
    }
}
