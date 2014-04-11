// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EntityArgumentsReader.cs" company="">
//   
// </copyright>
// <summary>
//   The entity arguments reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    using RabbitDB.Mapping;
    using RabbitDB.Reflection;

    /// <summary>
    /// The entity arguments reader.
    /// </summary>
    internal class EntityArgumentsReader
    {
        #region Methods

        /// <summary>
        /// The get entity arguments.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        /// <typeparam name="TEntity">
        /// </typeparam>
        /// <returns>
        /// The <see cref="object[]"/>.
        /// </returns>
        internal object[] GetEntityArguments<TEntity>(TEntity entity, TableInfo tableInfo)
        {
            var properties = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { entity });
            var count = properties.Count();

            var arguments = new List<KeyValuePair<string, object>>();
            for (var i = 0; i < count; i++)
            {
                var propertyInfo =
                    tableInfo.Columns.FirstOrDefault(column => column.ColumnAttribute.ColumnName == properties[i].Key);
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