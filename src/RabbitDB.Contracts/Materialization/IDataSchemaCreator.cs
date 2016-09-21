// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSchemaCreator.cs" company="">
//   
// </copyright>
// <summary>
//   The DataSchemaCreator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region using directives

using System.Data;

using RabbitDB.Contracts.Mapping;

#endregion

namespace RabbitDB.Contracts.Materialization
{
    /// <summary>
    ///     The DataSchemaCreator interface.
    /// </summary>
    internal interface IDataSchemaCreator
    {
        #region Public Methods

        /// <summary>
        ///     The column index.
        /// </summary>
        /// <param name="index">
        ///     The index.
        /// </param>
        /// <returns>
        ///     The <see cref="int" />.
        /// </returns>
        int ColumnIndex(int index);

        /// <summary>
        ///     The create from type.
        /// </summary>
        /// <param name="dataReader">
        ///     The data reader.
        /// </param>
        /// <param name="tableInfo">
        ///     The table info.
        /// </param>
        void CreateFromType(IDataReader dataReader, ITableInfo tableInfo);

        #endregion
    }
}