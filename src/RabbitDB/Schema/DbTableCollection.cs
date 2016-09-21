// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DbTableCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The db table collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections.ObjectModel;
using System.Linq;

#endregion

namespace RabbitDB.Schema
{
    /// <summary>
    ///     The db table collection.
    /// </summary>
    internal class DbTableCollection : Collection<DbTable>
    {
        #region  Properties

        /// <summary>
        ///     The
        /// </summary>
        /// <param name="tableName">
        ///     The table name.
        /// </param>
        /// <returns>
        ///     The <see cref="DbTable" />.
        /// </returns>
        internal DbTable this[string tableName] => GetTable(tableName);

        #endregion

        #region Internal Methods

        /// <summary>
        ///     The get table.
        /// </summary>
        /// <param name="tableName">
        ///     The table name.
        /// </param>
        /// <returns>
        ///     The <see cref="DbTable" />.
        /// </returns>
        internal DbTable GetTable(string tableName)
        {
            return this.SingleOrDefault(dbTable => string.Compare(dbTable.Name, tableName, StringComparison.OrdinalIgnoreCase) == 0);
        }

        #endregion
    }
}