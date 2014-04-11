﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyMetaInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The property meta info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Mapping
{
    using System;
    using System.Data;
    using System.Reflection;

    using RabbitDB.Attributes;
    using RabbitDB.Reflection;

    /// <summary>
    /// The property meta info.
    /// </summary>
    internal sealed class PropertyMetaInfo : MetaInfo
    {
        #region Fields

        /// <summary>
        /// The _property info.
        /// </summary>
        private readonly PropertyInfo _propertyInfo;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyMetaInfo"/> class.
        /// </summary>
        /// <param name="propertyInfo">
        /// The property info.
        /// </param>
        /// <param name="propertyType">
        /// The property type.
        /// </param>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="columnAttribute">
        /// The column attribute.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        /// <exception cref="TableInfoException">
        /// </exception>
        public PropertyMetaInfo(
            PropertyInfo propertyInfo, 
            Type propertyType, 
            DbType dbType, 
            ColumnAttribute columnAttribute)
            : base(propertyType, dbType, columnAttribute)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            if (!propertyInfo.CanWrite || !propertyInfo.CanRead)
            {
                if (propertyInfo.ReflectedType != null)
                {
                    throw new TableInfoException(
                        string.Format(
                            "Cannot create mapping for {0}.{1} because it's not possible to write and read.", 
                            propertyInfo.ReflectedType.FullName, 
                            propertyInfo.Name));
                }
            }

            _propertyInfo = propertyInfo;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns true if the MemberInfo is write-able.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return _propertyInfo.CanWrite;
            }
        }

        /// <summary>
        /// Returns the name of the element of the entity object type that is mapped to the field in the storage.
        /// </summary>
        public override string Name
        {
            get
            {
                return _propertyInfo.Name;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The get value.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <returns>
        /// The <see cref="object"/>.
        /// </returns>
        public override object GetValue(object obj)
        {
            return _propertyInfo.GetValueFast(obj);
        }

        /// <summary>
        /// The set value.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public override void SetValue(object obj, object value)
        {
            if (this.IsNullable && this.PropertyType.BaseType == typeof(Enum))
            {
                var type = typeof(Nullable<>).MakeGenericType(this.PropertyType);
                value = value == null
                            ? Activator.CreateInstance(type)
                            : Activator.CreateInstance(type, Enum.ToObject(this.PropertyType, value));
            }

            _propertyInfo.SetValue(
                obj, 
                value, 
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, 
                null, 
                null, 
                null);

            // _propertyInfo.SetValueFast(obj, value);
        }

        #endregion
    }
}