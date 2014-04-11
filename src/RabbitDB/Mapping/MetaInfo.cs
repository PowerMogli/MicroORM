// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MetaInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The meta info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Mapping
{
    using System;
    using System.Data;

    using RabbitDB.Attributes;

    /// <summary>
    /// The meta info.
    /// </summary>
    internal abstract class MetaInfo : IPropertyInfo
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaInfo"/> class.
        /// </summary>
        /// <param name="propertyType">
        /// The property type.
        /// </param>
        /// <param name="columnAttribute">
        /// The column attribute.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        internal MetaInfo(Type propertyType, ColumnAttribute columnAttribute)
        {
            if (propertyType == null)
            {
                throw new ArgumentNullException("propertyType");
            }

            this.PropertyType = propertyType;
            this.ColumnAttribute = columnAttribute;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetaInfo"/> class.
        /// </summary>
        /// <param name="propertyType">
        /// The property type.
        /// </param>
        /// <param name="dbType">
        /// The db type.
        /// </param>
        /// <param name="columnAttribute">
        /// The column attribute.
        /// </param>
        internal MetaInfo(Type propertyType, DbType dbType, ColumnAttribute columnAttribute)
            : this(propertyType, columnAttribute)
        {
            this.DbType = dbType;

            this.IsNullable = propertyType.IsGenericType
                              && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether can write.
        /// </summary>
        public abstract bool CanWrite { get; }

        /// <summary>
        /// Gets the column attribute.
        /// </summary>
        public ColumnAttribute ColumnAttribute { get; private set; }

        /// <summary>
        /// Gets or sets the db type.
        /// </summary>
        public DbType? DbType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is nullable.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Returns the name of the element of the persistent object type that is mapped to the field in the storage.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the property type.
        /// </summary>
        public Type PropertyType { get; private set; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        public int Size
        {
            get
            {
                return this.ColumnAttribute.Size;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Returns the value of the element of the persistent object type that is mapped to a field in the storage.
        /// </summary>
        /// <param name="obj">
        /// The persistent object that's value is returned.
        /// </param>
        /// <returns>
        /// Value of the persistent object's type element that is mapped to a field in the storage.
        /// </returns>
        public abstract object GetValue(object obj);

        /// <summary>
        /// Sets new value for the element of the persistent object type that is mapped to the storage.
        /// </summary>
        /// <param name="obj">
        /// The persistent object that's member's value is updated.
        /// </param>
        /// <param name="value">
        /// The new value.
        /// </param>
        public abstract void SetValue(object obj, object value);

        #endregion
    }
}