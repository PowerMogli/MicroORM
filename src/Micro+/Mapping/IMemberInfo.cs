using System;

namespace MicroORM.Mapping
{
    internal interface IMemberInfo
    {
        /// <summary>
        /// Returns the <see cref="FieldAttribute">FieldAttribute</see> that contains the mapping information of
        /// the storage field.
        /// </summary>
        FieldAttribute FieldAttribute { get; }

        /// <summary>
        /// Returns an array containing the custom attributes on the element that is mapped to the storage.
        /// </summary>
        /// <param name="inherit">True to get also inherited attributes.</param>
        /// <returns>An array containing the custom attribute of the element.</returns>
        object[] GetCustomAttributes(bool inherit);

        /// <summary>
        /// Returns the value of the element of the persistent object type that is mapped to a field in the storage.
        /// </summary>
        /// <param name="obj">The persistent object that's member's value is returned.</param>
        /// <returns>Value of the persistent object's type element that is mapped to a field in the storage.</returns>
        object GetValue(object obj);

        /// <summary>
        /// Returns whether the <see cref="IMemberInfo">IMemberInfo</see> object is representing an element that is a value
        /// type but is allowed to be null.
        /// </summary>
        bool IsNullable { get; }

        /// <summary>
        /// Returns the type of the element of the persistent object.
        /// </summary>
        Type MemberType { get; }

        /// <summary>
        /// Returns the name of the element of the persistent object type that is mapped to the field in the storage.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Sets new value for the element of the persistent object type that is mapped to the storage.
        /// </summary>
        /// <param name="obj">The persistent object that's member's value is updated.</param>
        /// <param name="value">The new value.</param>
        void SetValue(object obj, object value);
    }
}
