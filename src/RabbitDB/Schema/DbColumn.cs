// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbColumn.cs" company="">
//   
// </copyright>
// <summary>
//   The db column.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Data;

using RabbitDB.Contracts.Schema;

#endregion

namespace RabbitDB.Schema
{
    /// <summary>
    ///     The db column.
    /// </summary>
    internal class DbColumn : IDbColumn
    {
        #region  Properties

        /// <summary>
        ///     Gets or sets the db type.
        /// </summary>
        public DbType DbType { get; set; }

        /// <summary>
        ///     Gets or sets the default value.
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether ignore.
        /// </summary>
        public bool Ignore { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is auto increment.
        /// </summary>
        public bool IsAutoIncrement { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is computed.
        /// </summary>
        public bool IsComputed { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is nullable.
        /// </summary>
        public bool IsNullable { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether is primary key.
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        ///     Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Gets or sets the precision.
        /// </summary>
        public int Precision { get; set; }

        /// <summary>
        ///     Gets or sets the property name.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        ///     Gets or sets the size.
        /// </summary>
        public int Size { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The is to skip.
        /// </summary>
        /// <param name="resolvedColumnName">
        ///     The resolved column name.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool IsToSkip(string resolvedColumnName)
        {
            return Name == resolvedColumnName && (IsPrimaryKey || IsAutoIncrement);
        }

        #endregion
    }
}