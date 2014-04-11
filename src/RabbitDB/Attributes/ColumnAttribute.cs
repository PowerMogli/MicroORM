﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ColumnAttribute.cs" company="">
//   
// </copyright>
// <summary>
//   The column attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Attributes
{
    using System;
    using System.Data;

    /// <summary>
    /// The column attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ColumnAttribute : Attribute
    {
        #region Fields

        /// <summary>
        /// The _size.
        /// </summary>
        private int _size = -1;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        public ColumnAttribute()
        {
            this.IsNullable = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public ColumnAttribute(string columnName)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException("columnName");
            }

            this.ColumnName = columnName;
            this.IsNullable = true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether auto number.
        /// </summary>
        public bool AutoNumber { get; set; }

        /// <summary>
        /// Gets or sets the column name.
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// Gets or sets the db type.
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is nullable.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is primary key.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// Gets or sets the size.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// </exception>
        public int Size
        {
            get
            {
                return _size;
            }

            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("Value is below zero. Not allowed.");
                }

                _size = value;
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the null db type.
        /// </summary>
        internal DbType? NullDbType
        {
            get
            {
                return this.DbType;
            }
        }

        #endregion
    }
}