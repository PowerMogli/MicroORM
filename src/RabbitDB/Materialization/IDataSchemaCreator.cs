// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataSchemaCreator.cs" company="">
//   
// </copyright>
// <summary>
//   The DataSchemaCreator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace RabbitDB.Materialization
{
    using System.Data;

    using RabbitDB.Mapping;

    /// <summary>
    /// The DataSchemaCreator interface.
    /// </summary>
    interface IDataSchemaCreator
    {
        #region Public Methods and Operators

        /// <summary>
        /// The column index.
        /// </summary>
        /// <param name="index">
        /// The index.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int ColumnIndex(int index);

        /// <summary>
        /// The create from type.
        /// </summary>
        /// <param name="dataReader">
        /// The data reader.
        /// </param>
        /// <param name="tableInfo">
        /// The table info.
        /// </param>
        void CreateFromType(IDataReader dataReader, TableInfo tableInfo);

        #endregion
    }
}