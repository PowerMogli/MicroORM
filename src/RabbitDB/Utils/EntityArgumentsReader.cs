// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityArgumentsReader.cs" company="">
//   
// </copyright>
// <summary>
//   The entity arguments reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Collections.Generic;
using System.Linq;

using RabbitDB.Contracts.Mapping;
using RabbitDB.Mapping;
using RabbitDB.Reflection;

#endregion

namespace RabbitDB.Utils
{
    /// <summary>
    ///     The entity arguments reader.
    /// </summary>
    internal class EntityArgumentsReader
    {
        #region Internal Methods

        /// <summary>
        ///     The get entity arguments.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        ///     The <see cref="object[]" />.
        /// </returns>
        internal object[] GetEntityArguments<TEntity>(TEntity entity, TableInfo tableInfo)
        {
            KeyValuePair<string, object>[] properties = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { entity });
            int count = properties.Length;

            List<KeyValuePair<string, object>> arguments = new List<KeyValuePair<string, object>>();
            for (int i = 0; i < count; i++)
            {
                IPropertyInfo propertyInfo = tableInfo.Columns.FirstOrDefault(column => column.ColumnAttribute.ColumnName == properties[i].Key);
                if (propertyInfo == null
                    || (tableInfo.Columns.Contains(propertyInfo.ColumnAttribute.ColumnName)
                        && (propertyInfo.ColumnAttribute.AutoNumber || propertyInfo.ColumnAttribute.IsPrimaryKey)))
                {
                    continue;
                }

                arguments.Add(properties[i]);
            }

            return new object[] { arguments.ToArray() };
        }

        #endregion
    }
}