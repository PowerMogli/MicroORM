// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyInfoCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The property info collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using RabbitDB.Contracts.Mapping;
using RabbitDB.Contracts.Schema;
using RabbitDB.Contracts.SqlDialect;

#endregion

namespace RabbitDB.Mapping
{
    /// <summary>
    ///     The property info collection.
    /// </summary>
    internal sealed class PropertyInfoCollection : IPropertyInfoCollection
    {
        #region Fields

        /// <summary>
        ///     The _property infos.
        /// </summary>
        private readonly List<IPropertyInfo> _propertyInfos = new List<IPropertyInfo>();

        /// <summary>
        ///     The _property name member mapping.
        /// </summary>
        private readonly Dictionary<string, IPropertyInfo> _propertyNameMemberMapping =
            new Dictionary<string, IPropertyInfo>();

        #endregion

        #region  Properties

        /// <summary>
        ///     Gets the count.
        /// </summary>
        public int Count => _propertyInfos.Count;

        /// <summary>
        ///     The
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <returns>
        ///     The <see cref="IPropertyInfo" />.
        /// </returns>
        public IPropertyInfo this[int index] => _propertyInfos[index];

        #endregion

        #region Public Methods

        /// <summary>
        ///     The add.
        /// </summary>
        /// <param name="propertyInfo">
        ///     The property info.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public void Add(IPropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (_propertyNameMemberMapping.ContainsKey(propertyInfo.Name))
            {
                return;
            }

            _propertyNameMemberMapping.Add(propertyInfo.Name, propertyInfo);
            _propertyInfos.Add(propertyInfo);
        }

        /// <summary>
        ///     The contains.
        /// </summary>
        /// <param name="columnName">
        ///     The column name.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Contains(string columnName)
        {
            return _propertyInfos.Any(propertyInfo => propertyInfo.ColumnAttribute.ColumnName == columnName);
        }

        /// <summary>
        ///     The get enumerator.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        public IEnumerator GetEnumerator()
        {
            return _propertyInfos.GetEnumerator();
        }

        /// <summary>
        ///     The remove.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        public void Remove(string name)
        {
            IPropertyInfo propertyInfo = _propertyNameMemberMapping[name];
            _propertyNameMemberMapping.Remove(name);
            _propertyInfos.Remove(propertyInfo);
        }

        /// <summary>
        ///     The select valid column names.
        /// </summary>
        /// <param name="table">
        ///     The table.
        /// </param>
        /// <param name="sqlCharacters">
        ///     The sql characters.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<string> SelectValidColumnNames(IDbTable table, ISqlCharacters sqlCharacters)
        {
            return
                this.Where(column => table.DbColumns.Any(dbColumn => dbColumn.Name == column.ColumnAttribute.ColumnName))
                    .Select(member => sqlCharacters.EscapeName(member.ColumnAttribute.ColumnName));
        }

        /// <summary>
        ///     The select valid non auto number column names.
        /// </summary>
        /// <param name="sqlCharacters">
        ///     The sql characters.
        /// </param>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<string> SelectValidNonAutoNumberColumnNames(ISqlCharacters sqlCharacters)
        {
            return
                this.Where(column => !column.ColumnAttribute.AutoNumber)
                    .Select(column => sqlCharacters.EscapeName(column.ColumnAttribute.ColumnName));
        }

        /// <summary>
        ///     The select valid non auto number prefixed column names.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerable" />.
        /// </returns>
        public IEnumerable<string> SelectValidNonAutoNumberPrefixedColumnNames()
        {
            return this.Where(column => !column.ColumnAttribute.AutoNumber)
                       .Select(column => "@" + column.Name);
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     The get enumerator.
        /// </summary>
        /// <returns>
        ///     The <see cref="IEnumerator" />.
        /// </returns>
        IEnumerator<IPropertyInfo> IEnumerable<IPropertyInfo>.GetEnumerator()
        {
            return _propertyInfos.GetEnumerator();
        }

        #endregion
    }
}