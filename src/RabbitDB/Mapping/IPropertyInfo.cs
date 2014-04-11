// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPropertyInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The PropertyInfo interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Mapping
{
    using System;
    using System.Data;

    using RabbitDB.Attributes;

    /// <summary>
    /// The PropertyInfo interface.
    /// </summary>
    internal interface IPropertyInfo
    {
        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether can write.
        /// </summary>
        bool CanWrite { get; }

        /// <summary>
        /// Gets the column attribute.
        /// </summary>
        ColumnAttribute ColumnAttribute { get; }

        /// <summary>
        /// Gets or sets the db type.
        /// </summary>
        DbType? DbType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is nullable.
        /// </summary>
        bool IsNullable { get; set; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the property type.
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// Gets the size.
        /// </summary>
        int Size { get; }

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
        object GetValue(object obj);

        /// <summary>
        /// The set value.
        /// </summary>
        /// <param name="obj">
        /// The obj.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        void SetValue(object obj, object value);

        #endregion
    }
}