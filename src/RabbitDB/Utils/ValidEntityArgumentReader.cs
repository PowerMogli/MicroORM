// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidEntityArgumentReader.cs" company="">
//   
// </copyright>
// <summary>
//   The valid entity argument reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Collections.Generic;
using System.Linq;

using RabbitDB.Mapping;
using RabbitDB.Reflection;

#endregion

namespace RabbitDB.Utils
{
    /// <summary>
    ///     The valid entity argument reader.
    /// </summary>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal class ValidEntityArgumentReader<TEntity> : IValidEntityArgumentsReader
    {
        #region Fields

        /// <summary>
        ///     The _entity.
        /// </summary>
        private readonly TEntity _entity;

        #endregion

        #region Construction

        /// <summary>
        ///     Initializes a new instance of the <see cref="ValidEntityArgumentReader{TEntity}" /> class.
        /// </summary>
        /// <param name="entity">
        ///     The entity.
        /// </param>
        internal ValidEntityArgumentReader(TEntity entity)
        {
            _entity = entity;
        }

        #endregion

        #region Public Methods

        /// <summary>
        ///     The read valid entity arguments.
        /// </summary>
        /// <returns>
        ///     The
        ///     <see>
        ///         <cref>IEnumerable</cref>
        ///     </see>
        ///     .
        /// </returns>
        public IEnumerable<KeyValuePair<string, object>> ReadValidEntityArguments()
        {
            KeyValuePair<string, object>[] entityValues = ParameterTypeDescriptor.ToKeyValuePairs(new object[] { _entity });

            TableInfo tableInfo = TableInfo<TEntity>.GetTableInfo;

            return entityValues.Where(kvp => tableInfo.DbTable.DbColumns.Any(column => column.Name == tableInfo.ResolveColumnName(kvp.Key)));
        }

        #endregion
    }
}