using System;
using System.Reflection;

namespace MicroORM.Mapping
{
    /// <summary>
    /// Class represents a meta element that is used to create the mapping between the persistent object and
    /// the field in the table.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class represents a meta element that is used to create the mapping between a property or a field
    /// of the persistent object and a field in the storage table.
    /// </para>
    /// <para>
    /// ATTENTION: Named MetaInfo instead of MemberInfo to avoid naming clashes with System.Reflection.MemberInfo.
    /// </para>
    /// </remarks>
    internal abstract class MetaInfo : IMemberInfo
    {
        private FieldAttribute _attribute;
        private bool _isNullable;
        private Type _memberType;
        private MemberInfo _memberInfo;

        /// <summary>
        /// Creates a new instance of the <see cref="MetaInfo">MetaInfo Class</see>.
        /// </summary>
        /// <param name="memberType">The type of the MemberInfo object.</param>
        /// <param name="attribute">The <see cref="FieldAttribute">FieldAttribute</see> that contains the mapping information
        /// of the storage field.</param>
        /// <param name="memberInfo">The object holding all informations about the member.</param>
        internal MetaInfo(Type memberType, MemberInfo memberInfo, FieldAttribute attribute)
        {
            if (memberType == null)
                throw new ArgumentNullException("memberType");
            if (attribute == null)
                throw new ArgumentNullException("attribute");
            if (memberInfo == null)
                throw new ArgumentNullException("memberInfo");

            _attribute = attribute;
            _memberType = memberType;
            _memberInfo = memberInfo;

            if (_attribute.FieldName == null)
                _attribute.FieldName = memberInfo.Name;

            _isNullable = (memberType.IsGenericType && memberType.GetGenericTypeDefinition() == typeof(Nullable<>));
            if (_isNullable)
            {
                // Set the generic type of the nullable as type of the object.
                _memberType = memberType.GetGenericArguments()[0];
            }
        }

        /// <summary>
        /// Returns the <see cref="FieldAttribute">FieldAttribute</see> that contains the mapping information of
        /// the storage field.
        /// </summary>
        public FieldAttribute FieldAttribute
        {
            get { return _attribute; }
        }

        /// <summary>
        /// Returns whether the <see cref="IMemberInfo">IMemberInfo</see> object is representing an element that is a value
        /// type but is allowed to be null.
        /// </summary>
        public bool IsNullable
        {
            get { return _isNullable; }
        }

        /// <summary>
        /// Returns the type of the element of the persistent object.
        /// </summary>
        public Type MemberType
        {
            get { return _memberType; }
        }

        /// <summary>
        /// Returns an array containing the custom attributes on the element that is mapped to the storage.
        /// </summary>
        /// <param name="inherit">True to get also inherited attributes.</param>
        /// <returns>An array containing the custom attribute of the element.</returns>
        public object[] GetCustomAttributes(bool inherit)
        {
            return _memberInfo.GetCustomAttributes(inherit);
        }

        #region Abstract members

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

        #endregion
    }
}
