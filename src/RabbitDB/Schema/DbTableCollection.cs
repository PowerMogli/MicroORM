// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbTableCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The db table collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Schema
{
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// The db table collection.
    /// </summary>
    internal class DbTableCollection : Collection<DbTable>
    {
        #region Indexers

        /// <summary>
        /// The this.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="DbTable"/>.
        /// </returns>
        internal DbTable this[string tableName]
        {
            get
            {
                return this.GetTable(tableName);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The get table.
        /// </summary>
        /// <param name="tableName">
        /// The table name.
        /// </param>
        /// <returns>
        /// The <see cref="DbTable"/>.
        /// </returns>
        internal DbTable GetTable(string tableName)
        {
            return this.SingleOrDefault(dbTable => String.Compare(dbTable.Name, tableName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        #endregion
    }
}